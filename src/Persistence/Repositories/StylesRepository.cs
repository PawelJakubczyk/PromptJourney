using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class StylesRepository(MidjourneyDbContext dbContext, HybridCache cache) : IStyleRepository
{
    private const string _tagCacheKey = "tag";

    private readonly MidjourneyDbContext _midjourneyDbContext = dbContext;
    private readonly HybridCache _cache = cache;

    private readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        Expiration = TimeSpan.FromDays(365),
        LocalCacheExpiration = TimeSpan.FromDays(365)
    };

    // Query Methods
    public async Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync(CancellationToken cancellationToken)
    {
        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Include(s => s.Tags)
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<MidjourneyStyle>> GetStyleByNameAsync(StyleName name, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(s => s.StyleName == name, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(name));

        return Result.Ok(style);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type, CancellationToken cancellationToken)
    {
        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Include(s => s.Tags)
            .Where(s => s.Type == type)
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<Tag> tags, CancellationToken cancellationToken)
    {
        if (tags is null || tags.Count is 0) return Result.Ok(new List<MidjourneyStyle>());

        var tagValues = tags.Select(t => t.Value).ToList();

        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Include(s => s.Tags)
            .Where(s => s.Tags != null && s.Tags.Any(t => tagValues.Contains(t.Value)))
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTagsMatchAllAsync(List<Tag> tags, CancellationToken cancellationToken)
    {
        if (tags is null || tags.Count is 0) return Result.Ok(new List<MidjourneyStyle>());

        var tagValues = tags.Select(t => t.Value).ToList();

        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Include(s => s.Tags)
            .Where(s => s.Tags != null && tagValues.All(tv => s.Tags.Any(t => t.Value == tv)))
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Include(s => s.Tags)
            .Where(s => s.Description != null && EF.Functions.Like(s.Description.Value, $"%{keyword.Value}%"))
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<List<string>>> GetAllStyleTagsAsync(CancellationToken cancellationToken)
    {
        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.Tags)
            .Where(s => s.Tags != null && s.Tags.Any())
            .ToListAsync(cancellationToken);

        var allTags = await GetOrCreateCachedTagsAsync(cancellationToken);

        return Result.Ok(allTags);
    }

    public async Task<Result<bool>> CheckStyleExistsAsync(StyleName name, CancellationToken cancellationToken)
    {
        var exist = await _midjourneyDbContext.MidjourneyStyle
            .AnyAsync(s => s.StyleName == name, cancellationToken);

        return Result.Ok(exist);
    }

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(StyleName styleName, Tag tag, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<bool>(DomainErrors.StyleNotFound(styleName));

        var exists = style.Tags?.Any(t => t.Value == tag.Value) ?? false;
        return Result.Ok(exists);
    }

    // Command Methods
    public async Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        await _midjourneyDbContext.MidjourneyStyle.AddAsync(style, cancellationToken);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        // If style has tags, check cached tags and invalidate if new tag(s) were introduced
        if (style.Tags != null && style.Tags.Count > 0)
        {
            var cachedTags = await GetOrCreateCachedTagsAsync(cancellationToken);

            var newTagDetected = style.Tags
                .Select(t => t?.Value)
                .Where(v => !string.IsNullOrEmpty(v))
                .Any(v => !cachedTags.Contains(v));

            if (newTagDetected)
            {
                await InvalidateCacheAsync(cancellationToken);
            }
        }

        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        var existingStyle = await _midjourneyDbContext.MidjourneyStyle
            .FirstOrDefaultAsync(s => s.StyleName == style.StyleName, cancellationToken);

        if (existingStyle is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(style.StyleName));

        _midjourneyDbContext.Entry(existingStyle).CurrentValues.SetValues(style);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);
        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> DeleteStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        _midjourneyDbContext.MidjourneyStyle.Remove(style);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);
        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> AddTagToStyleAsync(StyleName styleName, Tag tagResult, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        style.AddTag(tagResult);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        var tags = await GetOrCreateCachedTagsAsync(cancellationToken);
        if (!tags.Contains(tagResult.Value)) await InvalidateCacheAsync(cancellationToken);
        
        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
    {
        if (tagResult.IsFailed)
        {
            throw new ArgumentException("Invalid tag provided", nameof(tagResult));
        }

        var style = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        style.RemoveTag(tagResult);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);
        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> UpdateStyleDescriptionAsync(StyleName styleName, Description description, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        style.EditDescription(description);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(style);
    }

    // Helper methods
    private async Task<List<string?>> GetOrCreateCachedTagsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync
        (
            _tagCacheKey,
            async (ct) =>
            {
                var tags = await _midjourneyDbContext.MidjourneyStyle
                    .Include(style => style.Tags)
                    .Where(style => style.Tags != null && style.Tags.Any())
                    .SelectMany(style => style.Tags!)
                    .Select(tag => tag.Value)
                    .Where(value => !string.IsNullOrEmpty(value))
                    .Distinct()
                    .ToListAsync(ct);

                return tags.Cast<string?>().ToList();
            },
            _cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(_tagCacheKey, cancellationToken);
    }
}
