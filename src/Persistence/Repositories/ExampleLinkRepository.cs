using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Utilities.Constants;
using Utilities.Errors;

namespace Persistence.Repositories;

public sealed class ExampleLinkRepository : IExampleLinksRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public ExampleLinkRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync(CancellationToken cancellationToken)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.VersionMaster)
                .ToListAsync(cancellationToken);

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get all example links: {ex.Message}");
            return Result.Fail<List<MidjourneyStyleExampleLink>>(error);
        }
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.VersionMaster)
                .Where(l => l.StyleName == styleName)
                .ToListAsync(cancellationToken);

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get example links by style: {ex.Message}");
            return Result.Fail<List<MidjourneyStyleExampleLink>>(error);
        }
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(
        StyleName styleName,
        ModelVersion version,
        CancellationToken cancellationToken)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.VersionMaster)
                .Where(l => l.StyleName == styleName && l.Version == version)
                .ToListAsync(cancellationToken);

            return Result.Ok(exampleLinks);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get example links by style and version: {ex.Message}");
            return Result.Fail<List<MidjourneyStyleExampleLink>>(error);
        }
    }

    public async Task<Result<bool>> CheckExampleLinkExistsAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(l => l.Link == link, cancellationToken);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to check if example link exists: {ex.Message}");
            return Result.Fail<bool>(error);
        }
    }

    public async Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(l => l.StyleName == styleName, cancellationToken);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to check if example link with style exists: {ex.Message}");
            return Result.Fail<bool>(error);
        }
    }

    public async Task<Result<bool>> CheckAnyExampleLinksExistAsync(CancellationToken cancellationToken)
    {
        try
        {
            var hasAnyLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(cancellationToken);

            return Result.Ok(hasAnyLinks);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to check if example links exist: {ex.Message}");
            return Result.Fail<bool>(error);
        }
    }

    public async Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink exampleLink, CancellationToken cancellationToken)
    {
        try
        {
            await _midjourneyDbContext.MidjourneyStyleExampleLinks.AddAsync(exampleLink, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(exampleLink);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to add example link: {ex.Message}");
            return Result.Fail<MidjourneyStyleExampleLink>(error);
        }
    }

    public async Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        try
        {
            var exampleLink = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .FirstOrDefaultAsync(l => l.Link == link, cancellationToken);

            if (exampleLink == null)
                return Result.Fail<MidjourneyStyleExampleLink>(new Error<PersistenceLayer>($"Example link not found: {link}"));

            _midjourneyDbContext.MidjourneyStyleExampleLinks.Remove(exampleLink);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(exampleLink);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to delete example link: {ex.Message}");
            return Result.Fail<MidjourneyStyleExampleLink>(error);
        }
    }

    public async Task<Result<int>> DeleteAllExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        try
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Where(l => l.StyleName == styleName)
                .ToListAsync(cancellationToken);

            if (!exampleLinks.Any())
                return Result.Fail<int>(new Error<PersistenceLayer>($"No example links found for style '{styleName}'"));

            _midjourneyDbContext.MidjourneyStyleExampleLinks.RemoveRange(exampleLinks);
            var deletedCount = await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(deletedCount);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to delete all example links by style: {ex.Message}");
            return Result.Fail<int>(error);
        }
    }
}
