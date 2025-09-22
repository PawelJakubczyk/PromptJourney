using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;

public sealed class StylesRepository : IStyleRepository
{
    private readonly MidjourneyDbContext _dbContext;

    public StylesRepository(MidjourneyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var styles = await _dbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync(cancellationToken);

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get all styles: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> GetStyleByNameAsync(StyleName name, CancellationToken cancellationToken)
    {
        try
        {
            var style = await _dbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == name, cancellationToken);

            return style is null
                ? Result.Fail<MidjourneyStyle>($"Style '{name.Value}' not found")
                : Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to get style: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type, CancellationToken cancellationToken)
    {
        try
        {
            var styles = await _dbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Type == type)
                .ToListAsync(cancellationToken);

            return styles.Count == 0
                ? Result.Fail<List<MidjourneyStyle>>($"No styles found for type '{type.Value}'")
                : Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by type: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<Tag> tags, CancellationToken cancellationToken)
    {
        try
        {
            var tagValues = tags.Select(t => t.Value).ToList();

            var styles = await _dbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Tags != null && tagValues.All(tag => s.Tags.Any(t => t.Value == tag)))
                .ToListAsync(cancellationToken);

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by tags: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        try
        {
            var styles = await _dbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Description != null && s.Description.Value.Contains(keyword.Value))
                .ToListAsync(cancellationToken);

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by description keyword: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckStyleExistsAsync(StyleName name, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _dbContext.MidjourneyStyle
                .AnyAsync(s => s.StyleName == name, cancellationToken);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to check if style exists: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(StyleName styleName, Tag tag, CancellationToken cancellationToken)
    {
        try
        {
            var style = await _dbContext.MidjourneyStyle
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            return style is null
                ? Result.Fail<bool>($"Style '{styleName.Value}' not found")
                : Result.Ok(style.Tags?.Any(t => t.Value == tag.Value) ?? false);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to check if tag exists: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(string tag, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _dbContext.MidjourneyStyle
                .AnyAsync(s => s.Tags != null && s.Tags.Any(t => t.Value == tag), cancellationToken);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to check if tag exists: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.MidjourneyStyle.AddAsync(style, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to add style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style, CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.MidjourneyStyle.Update(style);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to update style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        try
        {
            var style = await _dbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == styleName, cancellationToken);

            if (style is null)
                return Result.Fail<MidjourneyStyle>($"Style '{styleName.Value}' not found");

            _dbContext.MidjourneyStyle.Remove(style);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to delete style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> AddTagToStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
    {
        try
        {
            var style = await _dbContext.MidjourneyStyle.FindAsync([styleName], cancellationToken);
            if (style is null)
                return Result.Fail<MidjourneyStyle>($"Style '{styleName.Value}' not found");

            var result = style.AddTag(tagResult);
            if (result.IsFailed)
                return Result.Fail<MidjourneyStyle>(result.Errors);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to add tag: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Result<Tag> tagResult, CancellationToken cancellationToken)
    {
        try
        {
            var style = await _dbContext.MidjourneyStyle.FindAsync([styleName], cancellationToken);
            if (style is null)
                return Result.Fail<MidjourneyStyle>($"Style '{styleName.Value}' not found");

            var result = style.RemoveTag(tagResult);
            if (result.IsFailed)
                return Result.Fail<MidjourneyStyle>(result.Errors);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to remove tag: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> UpdateStyleDescriptionAsync(StyleName styleName, Description description, CancellationToken cancellationToken)
    {
        try
        {
            var style = await _dbContext.MidjourneyStyle.FindAsync([styleName], cancellationToken);
            if (style is null)
                return Result.Fail<MidjourneyStyle>($"Style '{styleName.Value}' not found");

            var result = style.EditDescription(description);
            if (result.IsFailed)
                return Result.Fail<MidjourneyStyle>(result.Errors);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to update description: {ex.Message}");
        }
    }
}
