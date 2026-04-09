using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Persistence.Context;
using Utilities.Results;

namespace Persistence.Repositories;

public sealed class StylesRepository(MidjourneyDbContext dbContext, HybridCache cache) : IStyleRepository
{
    private const string _tagCacheKey = "tag";
    private const string _StyleCacheKey = "style";
    private const string _tagSupportedCacheKey = "supported_tag";

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
        var styles = await _midjourneyDbContext.MidjourneyStyle.ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<MidjourneyStyle>> GetStyleByNameAsync(StyleName name, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .FirstOrDefaultAsync(s => s.StyleName == name, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(name));

        return Result.Ok(style);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type, CancellationToken cancellationToken)
    {
        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Where(s => s.Type == type)
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTags(TagsCollection tags, CancellationToken cancellationToken)
    {
        if (tags.IsNone) return Result.Ok(new List<MidjourneyStyle>());

        var tagValues = tags.CollectionValues.ToArray();
       
        var styles = _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .AsEnumerable()
            .Where(s => s.Tags != null && s.Tags.CollectionValues.Any(t => tagValues.Contains(t)))
            .ToList();

        return Result.Ok(styles);
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        var styles = await _midjourneyDbContext.MidjourneyStyle
            .Include(s => s.MidjourneyExampleLinks)
            .Where(s => s.Description != null && EF.Functions.Like(s.Description.Value, $"%{keyword.Value}%"))
            .ToListAsync(cancellationToken);

        return Result.Ok(styles);
    }

    public async Task<Result<List<Tag>>> GetAllStyleTagsAsync(CancellationToken cancellationToken)
    {
        // Use cached full Tag objects
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
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<bool>(DomainErrors.StyleNotFound(styleName));

        if (tag.IsNone)
        {
            return Result.Fail<bool>(DomainErrors.TagNotFound(tag));
        }

        var exists = !style.Tags.IsNone && style.Tags.Value.Any(t => t.Value == tag.Value);
        return Result.Ok(exists);
    }

    // Command Methods
    public async Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        await _midjourneyDbContext.MidjourneyStyle.AddAsync(style, cancellationToken);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        // If style has tags, check cached supported tag values and invalidate if new tag(s) were introduced
        if (!style.Tags.IsNone && style.Tags.Value.Count > 0)
        {
            var cachedSupportedTagValues = await GetOrCreateCachedSupportedTagsAsync(cancellationToken);

            var newTagDetected = style.Tags.Value
                .Select(t => t?.Value)
                .Where(v => !string.IsNullOrEmpty(v))
                .Any(v => !cachedSupportedTagValues.Contains(v!));

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
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        style.AddTag(tagResult);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        // Compare by tag values to avoid entity-tracking/equality pitfalls
        var supportedTagValues = await GetOrCreateCachedSupportedTagsAsync(cancellationToken);
        if (!supportedTagValues.Contains(tagResult.Value))
            await InvalidateCacheAsync(cancellationToken);

        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
    {
        if (tagResult.IsFailed)
        {
            throw new ArgumentException("Invalid tag provided", nameof(tagResult));
        }

        var style = await _midjourneyDbContext.MidjourneyStyle
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        // Extract value and remove by Tag (avoid passing Result<Tag> into domain method)
        var tag = tagResult.Value;
        style.RemoveTag(tag);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);
        return Result.Ok(style);
    }

    public async Task<Result<MidjourneyStyle>> UpdateStyleDescriptionAsync(StyleName styleName, Description description, CancellationToken cancellationToken)
    {
        var style = await _midjourneyDbContext.MidjourneyStyle
            .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

        if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

        style.UpdateDescription(description);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(style);
    }

    // Helper methods
    private async Task<List<Tag>> GetOrCreateCachedTagsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync<List<Tag>>
        (
            _tagCacheKey,
            async (ct) =>
            {
                var tags = await _midjourneyDbContext.MidjourneyStyle
                    .Where(style => style.Tags != null && !style.Tags.IsNone && style.Tags.Value.Count != 0)
                    .SelectMany(style => style.Tags.Value)
                    .Select(tag => tag)
                    .Where(value => value != null)
                    .Distinct()
                    .ToListAsync(ct);

                return [.. tags.Cast<Tag?>()];
            },
            _cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task<List<string>> GetOrCreateCachedSupportedTagsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync<List<string>>
        (
            _tagSupportedCacheKey,
            async (ct) =>
            {
                var styles = await _midjourneyDbContext.MidjourneyStyle
                .AsNoTracking()
                .Select(style => new { style.StyleName, style.Tags })
                .ToListAsync(ct);

                var tags = styles
                    .Where(style => !style.Tags.IsNone && style.Tags.Value.Count != 0)
                    .SelectMany(style => style.Tags.Value)
                    .Select(tag => tag.Value)
                    .Where(value => !string.IsNullOrEmpty(value))
                    .Distinct()
                    .ToList();

                return tags!;
            },
            _cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(_tagCacheKey, cancellationToken);
        await _cache.RemoveAsync(_tagSupportedCacheKey, cancellationToken);
    }
}
