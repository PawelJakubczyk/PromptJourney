using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class ExampleLinkRepository : IExampleLinksRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public ExampleLinkRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    // For Queries
    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync()
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.VersionMaster)
                .ToListAsync();

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyleExampleLink>>($"Failed to get all example links: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(string styleName)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.VersionMaster)
                .Where(l => l.StyleName == styleName)
                .ToListAsync();

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyleExampleLink>>($"Failed to get example links by style: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(string styleName, string version)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.VersionMaster)
                .Where(l => l.StyleName == styleName && l.Version == version)
                .ToListAsync();

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyleExampleLink>>($"Failed to get example links by style and version: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckExampleLinkExistsAsync(string link)
    {
        try
        {
            var exists = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(l => l.Link == link);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to check if example link exists: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(string styleName)
    {
        try
        {
            var exists = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(l => l.StyleName == styleName);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to check if example link with style exists: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckExampleLinksAreNotEmpty()
    {
        try
        {
            var hasAnyLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync();

            return Result.Ok(!hasAnyLinks);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Failed to check if example links are empty: {ex.Message}");
        }
    }

    // For Commands
    public async Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink link)
    {
        try
        {
            await _midjourneyDbContext.MidjourneyStyleExampleLinks.AddAsync(link);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(link);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyleExampleLink>($"Failed to add example link: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(string link)
    {
        try
        {
            var exampleLink = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .FirstOrDefaultAsync(l => l.Link == link);

            if (exampleLink is null)
                return Result.Fail<MidjourneyStyleExampleLink>($"Example link '{link}' not found");

            _midjourneyDbContext.MidjourneyStyleExampleLinks.Remove(exampleLink);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(exampleLink);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyleExampleLink>($"Failed to delete example link: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> DeleteAllExampleLinkByStyleAsync(string styleName)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Where(l => l.StyleName == styleName)
                .ToListAsync();

            if (exampleLinks.Count == 0)
                return Result.Fail<List<MidjourneyStyleExampleLink>>($"No example links found for style '{styleName}'");

            _midjourneyDbContext.MidjourneyStyleExampleLinks.RemoveRange(exampleLinks);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyleExampleLink>>($"Failed to delete all example links by style: {ex.Message}");
        }
    }
}