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

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.Version)
                .ToListAsync(cancellationToken);
        }, "Failed to get all example links", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.Version)
                .Where(l => l.StyleName == styleName)
                .ToListAsync(cancellationToken);
        }, "Failed to get example links by style", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(
        StyleName styleName,
        ModelVersion version,
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Include(l => l.Style)
                .Include(l => l.Version)
                .Where(l => l.StyleName == styleName && l.Version == version)
                .ToListAsync(cancellationToken);
        }, "Failed to get example links by style and version", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckExampleLinkExistsAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(l => l.Link == link, cancellationToken);
        }, "Failed to check if example link exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .AnyAsync(l => l.StyleName == styleName, cancellationToken);
        }, "Failed to check if example link with style exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckAnyExampleLinksExistAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyStyleExampleLinks.AnyAsync(cancellationToken);
        }, "Failed to check if example links exist", StatusCodes.Status500InternalServerError);
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

    public async Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var exampleLink = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .FirstOrDefaultAsync(l => l.Link == link, cancellationToken);

            _midjourneyDbContext.MidjourneyStyleExampleLinks.Remove(exampleLink!);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return exampleLink!;
        }, "Failed to delete example link", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<int>> DeleteAllExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
                .Where(l => l.StyleName == styleName)
                .ToListAsync(cancellationToken);

            _midjourneyDbContext.MidjourneyStyleExampleLinks.RemoveRange(exampleLinks);
            var deletedCount = await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return deletedCount;
        }, "Failed to delete all example links by style", StatusCodes.Status500InternalServerError);
    }
}