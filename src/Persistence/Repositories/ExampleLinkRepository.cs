using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class ExampleLinkRepository(MidjourneyDbContext midjourneyDbContext) : IExampleLinksRepository
    {
    private readonly MidjourneyDbContext _midjourneyDbContext = midjourneyDbContext;

    public Task<Result<bool>> CheckAnyExampleLinksExistAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks.AnyAsync(cancellationToken);
        }, "Failed to check if example links exist", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckExampleLinkExistsByLinkAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(exampleLink => exampleLink.Link == link, cancellationToken);

            return exist;
        }, "Failed to check if example link exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckExampleLinkExistsByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(exampleLink => exampleLink.Id == Id, cancellationToken);

            return exist;
        }, "Failed to check if example link exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckExampleLinkExistsByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(exampleLink => exampleLink.StyleName == styleName, cancellationToken);

            return exist;
        }, "Failed to check if example link with style exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(exampleLink => exampleLink.MidjuorneyStyle)
                .Include(exampleLink => exampleLink.MidjourneyMaster)
                .ToListAsync(cancellationToken);
        }, "Failed to get all example links", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyleExampleLink>> GetExampleLinkByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(exampleLink => exampleLink.MidjuorneyStyle)
                .Include(exampleLink => exampleLink.MidjourneyMaster)
                .FirstOrDefaultAsync(exampleLink => exampleLink.Id == id, cancellationToken)
                ?? throw new KeyNotFoundException($"Example link with ID {id} not found");
        }, "Failed to get example link by ID", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(exampleLink => exampleLink.MidjuorneyStyle)
                .Include(exampleLink => exampleLink.MidjourneyMaster)
                .Where(exampleLink => exampleLink.StyleName == styleName)
                .ToListAsync(cancellationToken);
        }, "Failed to get example links by style", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinkByLinkAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(exampleLink => exampleLink.MidjuorneyStyle)
                .Include(exampleLink => exampleLink.MidjourneyMaster)
                .Where(exampleLink => exampleLink.Link == link)
                .ToListAsync(cancellationToken);
        }, "Failed to get example links by link", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync
    (
        StyleName styleName,
        ModelVersion version,
        CancellationToken cancellationToken
    )
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(exampleLink => exampleLink.MidjuorneyStyle)
                .Include(exampleLink => exampleLink.MidjourneyMaster)
                .Where(exampleLink => exampleLink.StyleName == styleName && exampleLink.Version == version)
                .ToListAsync(cancellationToken);
        }, "Failed to get example links by style and version", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink exampleLink, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            await _midjourneyDbContext.MidjourneyStyleExampleLinks.AddAsync(exampleLink, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return exampleLink;
        }, "Failed to add example link", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<int>> DeleteAllExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Where(exampleLink => exampleLink.StyleName == styleName)
                .ToListAsync(cancellationToken);

            _midjourneyDbContext.MidjourneyStyleExampleLinks.RemoveRange(exampleLinks);
            var deletedCount = await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return deletedCount;
        }, "Failed to delete all example links by style", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var exampleLink = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (exampleLink == null) {
                throw new KeyNotFoundException($"Example link with ID {id} not found");
            }

            _midjourneyDbContext.MidjourneyStyleExampleLinks.Remove(exampleLink);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return exampleLink;
        }, "Failed to delete example link", StatusCodes.Status500InternalServerError);
    }
}
