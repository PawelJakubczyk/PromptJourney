using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class StylesRepository(MidjourneyDbContext dbContext) : IStyleRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext = dbContext;

    // Query Methods
    public Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .Include(s => s.Tags)
                .ToListAsync(cancellationToken);
        }, "Failed to get all styles", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> GetStyleByNameAsync(StyleName name, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.StyleName == name, cancellationToken);

            if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(name));

            return Result.Ok(style);
        }, $"Failed to get style '{name.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .Include(s => s.Tags)
                .Where(s => s.Type == type)
                .ToListAsync(cancellationToken);
        }, $"Failed to get styles by type '{type.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<Tag> tags, CancellationToken cancellationToken)
    {
        if (tags is null || tags.Count is 0) return Task.FromResult(Result.Ok(new List<MidjourneyStyle>()));
        
        var tagValues = tags.Select(t => t.Value).ToList();

        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .Include(s => s.Tags)
                .Where(s => s.Tags != null && s.Tags.Any(t => tagValues.Contains(t.Value)))
                .ToListAsync(cancellationToken);
        }, "Failed to get styles by tags", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByTagsMatchAllAsync(List<Tag> tags, CancellationToken cancellationToken)
    {
        if (tags == null || tags.Count == 0)
        {
            return Task.FromResult(Result.Ok(new List<MidjourneyStyle>()));
        }

        var tagValues = tags.Select(t => t.Value).ToList();

        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .Include(s => s.Tags)
                .Where(s => s.Tags != null && tagValues.All(tv => s.Tags.Any(t => t.Value == tv)))
                .ToListAsync(cancellationToken);
        }, "Failed to get styles by tags (match all)", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .Include(s => s.Tags)
                .Where(s => s.Description != null && EF.Functions.Like(s.Description.Value, $"%{keyword.Value}%"))
                .ToListAsync(cancellationToken);

            return styles;
        }, $"Failed to get styles by description keyword '{keyword.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<string>>> GetAllStyleTagsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.Tags)
                .Where(s => s.Tags != null && s.Tags.Any())
                .ToListAsync(cancellationToken);

            var allTags = styles
                .SelectMany(s => s.Tags ?? [])
                .Select(t => t.Value ?? string.Empty)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToList();

            return allTags;
        }, "Failed to get all style tags", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckStyleExistsAsync(StyleName name, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () => 
        {
            var exist = await _midjourneyDbContext.MidjourneyStyle
                .AnyAsync(s => s.StyleName == name, cancellationToken);

            return exist;
        }, $"Failed to check if style '{name.Value}' exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckTagExistsInStyleAsync(StyleName styleName, Tag tag, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () => 
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            if (style is null) return Result.Fail<bool>(DomainErrors.StyleNotFound(styleName));

            var exists = style.Tags?
                .Any(t => t.Value == tag.Value) ?? false;
            return Result.Ok(exists);
        }, $"Failed to check if tag '{tag.Value}' exists in style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckTagExistsInAnyStyleAsync(string tag, CancellationToken cancellationToken) 
    {
        return ExecuteAsync(async () => {
            var exist = await _midjourneyDbContext.MidjourneyStyle
                .AnyAsync(s => s.Tags != null && s.Tags
                .Any(t => t.Value == tag), cancellationToken);

            return exist;
        }, $"Failed to check if tag '{tag}' exists in any style", StatusCodes.Status500InternalServerError);
    }

    // Command Methods
    public Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () => {
            await _midjourneyDbContext.MidjourneyStyle.AddAsync(style, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to add style '{style.StyleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var existingStyle = await _midjourneyDbContext.MidjourneyStyle
                .FirstOrDefaultAsync(s => s.StyleName == style.StyleName, cancellationToken);

            if (existingStyle is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(style.StyleName));

            _midjourneyDbContext.Entry(existingStyle).CurrentValues.SetValues(style);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(style);
        }, $"Failed to update style '{style.StyleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> DeleteStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.MidjourneyExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);
            
            if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

            _midjourneyDbContext.MidjourneyStyle.Remove(style);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(style);
        }, $"Failed to delete style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> AddTagToStyleAsync(StyleName styleName, Tag tagResult, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

            style.AddTag(tagResult);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(style);
        }, $"Failed to add tag to style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
        {
        return ExecuteAsync(async () =>
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

            return Result.Ok(style);
        }, $"Failed to remove tag from style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> UpdateStyleDescriptionAsync(StyleName styleName, Description description, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            if (style is null) return Result.Fail<MidjourneyStyle>(DomainErrors.StyleNotFound(styleName));

            style.EditDescription(description);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(style);
        }, $"Failed to update description for style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }
}
