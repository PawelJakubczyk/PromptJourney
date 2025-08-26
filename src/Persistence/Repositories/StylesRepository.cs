using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
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

    public async Task<Result<MidjourneyStyle>> GetStyleByNameAsync(string name)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == name);

            return Result.Ok<MidjourneyStyle>(style!);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Failed to get style: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(string type)
    {
        try
        {
            await Validate.Type.ShouldBeNotNullOrEmpty(type);

            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Type == type)
                .ToListAsync();


            if (styles is null || styles.Count == 0)
            {
                return Result.Fail<List<MidjourneyStyle>>($"No styles found for type: '{type}'");
            }

            return Result.Ok<List<MidjourneyStyle>>(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by type: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<string> tags)
    {
        try
        {
            await Validate.Tags.ShouldHaveAtLastOneElement(tags);
            await Validate.Tags.ShouldNotHaveNullOrEmptyTag(tags);

            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Tags != null && tags.All(t => s.Tags.Contains(t)))
                .ToListAsync();

            return Result.Ok<List<MidjourneyStyle>>(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Failed to get styles by tags: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(string keyword)
    {
        try
        {
            await Validate.Keyword.ShouldBeNotNullOrEmpty(keyword);

            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Description != null && s.Description.Contains(keyword))
                .ToListAsync();

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to get styles by description keyword: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckStyleExistsAsync(string name)
    {
        try
        {
            await Validate.Style.ShouldBeNotNullOrEmpty(name);

            var exists = await _midjourneyDbContext.MidjourneyStyle.AnyAsync(s => s.StyleName == name);
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to check if style exists: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(string tag)
    {
        try
        {
            await Validate.Tag.ShouldBeNotNullOrEmpty(tag);

            var exists = await _midjourneyDbContext.MidjourneyStyle.AnyAsync(s => s.Tags != null && s.Tags.Contains(tag));
            return Result.Ok(exists);
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
            await Validate.Style.ShouldBeNotNull(style);
            await Validate.Style.NameAndTypeShouldNotBeNullOrEmpty(style);

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
            await Validate.Style.ShouldBeNotNull(style);
            await Validate.Style.NameAndTypeShouldNotBeNullOrEmpty(style);

            _midjourneyDbContext.MidjourneyStyle.Update(style);
            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to update style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteStyleAsync(string styleName)
    {
        try
        {
            await Validate.Style.ShouldBeNotNullOrEmpty(styleName);

            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.StyleName == styleName);

            if (style is null)
                return Result.Fail($"Style with name '{styleName}' not found");

            _midjourneyDbContext.MidjourneyStyle.Remove(style);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to delete style: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> AddTagToStyleAsync(string styleName, string tag)
    {
        try
        {
            await Validate.Style.ShouldBeNotNullOrEmpty(styleName);
            await Validate.Tag.ShouldBeNotNullOrEmpty(tag);

            var style = await _midjourneyDbContext
                .MidjourneyStyle
                .FirstOrDefaultAsync(s => s.StyleName == styleName);


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

    public async Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(string styleName, string tag)
    {
        try
        {
            await Validate.Style.ShouldBeNotNullOrEmpty(styleName);
            await Validate.Tag.ShouldBeNotNullOrEmpty(tag);

            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.StyleName == styleName);
            if (style is null)
                return Result.Fail($"Style with name '{styleName}' not found");

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

    public async Task<Result<bool>> CheckTagExistsInStyleAsync(string styleName, string tag)
    {
        try
        {
            await Validate.Style.ShouldBeNotNullOrEmpty(styleName);
            await Validate.Tag.ShouldBeNotNullOrEmpty(tag);

            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.StyleName == styleName);
            if (style is null)
                return Result.Fail($"Style with name '{styleName}' not found");

            return Result.Ok(style.Tags?.Contains(tag) ?? false);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to check if tag exists: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyle>> UpadteStyleDescription(string styleName, string description)
    {
        try
        {
            await Validate.Style.ShouldBeNotNullOrEmpty(styleName);

            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.StyleName == styleName);
            if (style is null)
                return Result.Fail($"Style with name '{styleName}' not found");

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