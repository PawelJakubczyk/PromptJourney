using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using System;

namespace Persistance.Repositories;

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

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to get all styles: {ex.Message}"));
        }
    }

    public async Task<Result<MidjourneyStyle>> GetStyleByNameAsync(string name)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.Name == name);

            if (style is null)
                return Result.Fail(new Error($"Style with name '{name}' not found"));

            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to get style: {ex.Message}"));
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(string type)
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Type == type)
                .ToListAsync();

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to get styles by type: {ex.Message}"));
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<string> tags)
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Tags != null && tags.All(t => s.Tags.Contains(t)))
                .ToListAsync();

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to get styles by tags: {ex.Message}"));
        }
    }

    public async Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(string keyword)
    {
        try
        {
            var styles = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .Where(s => s.Description != null && s.Description.Contains(keyword))
                .ToListAsync();

            return Result.Ok(styles);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to get styles by description keyword: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> CheckStyleExistsAsync(string name)
    {
        try
        {
            var exists = await _midjourneyDbContext.MidjourneyStyle.AnyAsync(s => s.Name == name);
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to check if style exists: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> CheckTagExistsAsync(string tag)
    {
        try
        {
            var exists = await _midjourneyDbContext.MidjourneyStyle.AnyAsync(s => s.Tags != null && s.Tags.Contains(tag));
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to check if tag exists: {ex.Message}"));
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
            return Result.Fail(new Error($"Failed to add style: {ex.Message}"));
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
            return Result.Fail(new Error($"Failed to update style: {ex.Message}"));
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteStyleAsync(string styleName)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle
                .Include(s => s.ExampleLinks)
                .FirstOrDefaultAsync(s => s.Name == styleName);

            if (style is null)
                return Result.Fail(new Error($"Style with name '{styleName}' not found"));

            _midjourneyDbContext.MidjourneyStyle.Remove(style);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to delete style: {ex.Message}"));
        }
    }

    public async Task<Result<MidjourneyStyle>> AddTagToStyleAsync(string styleName, string tag)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.Name == styleName);
            if (style is null)
                return Result.Fail(new Error($"Style with name '{styleName}' not found"));

            var result = style.AddTag(tag);
            if (result!.IsFailed)
                return Result.Fail(result.Errors);

            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to add tag to style: {ex.Message}"));
        }
    }

    public async Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(string styleName, string tag)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.Name == styleName);
            if (style is null)
                return Result.Fail(new Error($"Style with name '{styleName}' not found"));

            var result = style.RemoveTag(tag);
            if (result!.IsFailed)
                return Result.Fail(result.Errors);

            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to remove tag from style: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> CheckTagExistsAsync(string styleName, string tag)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.Name == styleName);
            if (style is null)
                return Result.Fail(new Error($"Style with name '{styleName}' not found"));

            return Result.Ok(style.Tags?.Contains(tag) ?? false);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to check if tag exists: {ex.Message}"));
        }
    }

    public async Task<Result<MidjourneyStyle>> UpadteStyleDescription(string styleName, string description)
    {
        try
        {
            var style = await _midjourneyDbContext.MidjourneyStyle.FirstOrDefaultAsync(s => s.Name == styleName);
            if (style is null)
                return Result.Fail(new Error($"Style with name '{styleName}' not found"));

            var result = style.EditDescription(description);
            if (result!.IsFailed)
                return Result.Fail(result.Errors);

            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(style);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to update style description: {ex.Message}"));
        }
    }
}