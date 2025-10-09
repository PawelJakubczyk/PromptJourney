using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using static Persistence.Repositories.Helper.RepositoryHelper;

public sealed class StylesRepository : IStyleRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public StylesRepository(MidjourneyDbContext dbContext)
    {
        _midjourneyDbContext = dbContext;
    }

    // Query Methods
    public async Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync(CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync(cancellationToken);
        }, "Failed to get all styles", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle?>> GetStyleByNameAsync(StyleName name, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == name, cancellationToken);
        }, $"Failed to get style '{name.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Type == type)
                .ToListAsync(cancellationToken);
        }, $"Failed to get styles by type '{type.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<Tag> tags, CancellationToken cancellationToken)
    {
        var tagValues = tags.Select(t => t.Value).ToList();

        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Tags != null && tagValues.All(tv => s.Tags.Any(t => t.Value == tv)))
                .ToListAsync(cancellationToken);
        }, "Failed to get styles by tags", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Description != null && EF.Functions.Like(s.Description.Value, $"%{keyword.Value}%"))
                .ToListAsync(cancellationToken);
        }, $"Failed to get styles by description keyword '{keyword.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckStyleExistsAsync(StyleName name, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle.AnyAsync(s => s.StyleName == name, cancellationToken);
        }, $"Failed to check if style '{name.Value}' exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckTagExistsInStyleAsync(StyleName styleName, Tag tag, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            return style.Tags?.Any(t => t.Value == tag.Value) ?? false;
        }, $"Failed to check if tag exists in style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckTagExistsInStyleAsync(string tag, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyle
                .AnyAsync(s => s.Tags != null && s.Tags.Any(t => t.Value == tag), cancellationToken);
        }, $"Failed to check if tag '{tag}' exists", StatusCodes.Status500InternalServerError);
    }

    // Command Methods
    public Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            await _midjourneyDbContext.MidjourneyStyle.AddAsync(style, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to add style '{style.StyleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            _midjourneyDbContext.MidjourneyStyle.Update(style);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to update style '{style.StyleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> DeleteStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            _midjourneyDbContext.MidjourneyStyle.Remove(style);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to delete style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> AddTagToStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            style!.AddTag(tagResult);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to add tag to style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            style!.RemoveTag(tagResult);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to remove tag from style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyle>> UpdateStyleDescriptionAsync(StyleName styleName, Description description, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            style!.EditDescription(description);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return style;
        }, $"Failed to update description for style '{styleName.Value}'", StatusCodes.Status500InternalServerError);
    }

}
