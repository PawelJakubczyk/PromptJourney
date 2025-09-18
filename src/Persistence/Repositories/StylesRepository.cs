using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class StylesRepository : IStyleRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;
    public StylesRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    public async Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync()
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync();

            return Result.Ok<List<MidjourneyStyle>>(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get all styles: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> GetStyleByNameAsync(StyleName name)
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync();
            
            var style = styles.FirstOrDefault(s => s.StyleName.Value == name.Value);

            if (style == null)
                return Result.Fail<MidjourneyStyle>($"Style with name '{name.Value}' not found");

            return Result.Ok<MidjourneyStyle>(style);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to get style: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type)
    {
        try
        {
            var allStyles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync();
            
            var styles = allStyles.Where(s => s.Type.Value == type.Value).ToList();

            if (styles is null || styles.Count == 0)
            {
                return Result.Fail<List<MidjourneyStyle>>($"No styles found for type: '{type.Value}'");
            }

            return Result.Ok<List<MidjourneyStyle>>(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by type: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<Tag> tags)
    {
        try
        {
            var tagValues = tags.Select(t => t.Value).ToArray();
            
            var allStyles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync();
                
            var styles = allStyles.Where(s => 
                s.Tags != null && tagValues.All(tagValue => 
                    s.Tags.Any(t => t.Value == tagValue))).ToList();

            return Result.Ok<List<MidjourneyStyle>>(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by tags: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword)
    {
        try
        {
            var allStyles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync();
                
            var styles = allStyles.Where(s => 
                s.Description != null && 
                s.Description.Value.Contains(keyword.Value)).ToList();

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to get styles by description keyword: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckStyleExistsAsync(StyleName name)
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle.ToListAsync();
            var exists = styles.Any(s => s.StyleName.Value == name.Value);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in CheckStyleExistsAsync: {ex}");
            return Result.Fail($"Failed to check if style exists: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(StyleName styleName, Tag tag)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FindAsync(styleName);
            if (style is null)
                return Result.Fail($"Style with name '{styleName.Value}' not found");

            return Result.Ok(style.Tags?.Any(t => t.Value == tag.Value) ?? false);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to check if tag exists: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style)
    {
        try
        {
            await _midjourneyDbContext.MidjourneyStyle.AddAsync(style);
            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to add style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style)
    {
        try
        {
            _midjourneyDbContext.MidjourneyStyle.Update(style);
            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to update style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteStyleAsync(StyleName styleName)
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .ToListAsync();
                
            var style = styles.FirstOrDefault(s => s.StyleName.Value == styleName.Value);

            if (style is null)
                return Result.Fail($"Style with name '{styleName.Value}' not found");

            _midjourneyDbContext.MidjourneyStyle.Remove(style);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to delete style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> AddTagToStyleAsync(StyleName styleName, Result<Tag> tag)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FindAsync(styleName);

            if (style is null)
                return Result.Fail($"Style with name '{styleName.Value}' not found");

            var result = style.AddTag(tag);
            if (result!.IsFailed)
                return Result.Fail(result.Errors);

            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to add tag to style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Result<Tag> tag)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FindAsync(styleName);
            if (style is null)
                return Result.Fail($"Style with name '{styleName.Value}' not found");

            var result = style.RemoveTag(tag);
            if (result!.IsFailed)
                return Result.Fail(result.Errors);

            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to remove tag from style: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(string tag)
    {
        try
        {
            var allStyles = await _midjourneyDbContext.MidjourneyStyle.ToListAsync();
            var exists = allStyles.Any(s => s.Tags != null && s.Tags.Any(t => t.Value == tag));
            
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to check if tag exists: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> UpadteStyleDescription(StyleName styleName, Description description)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FindAsync(styleName);
            if (style is null)
                return Result.Fail($"Style with name '{styleName.Value}' not found");

            var result = style.EditDescription(description);
            if (result!.IsFailed)
                return Result.Fail(result.Errors);

            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to update style description: {ex.Message}");
        }
    }
}