using Application.Abstractions.IRepository;
using Application.UseCases.Common.Responses;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class ExampleLinkRepository(MidjourneyDbContext midjourneyDbContext, IVersionRepository versionsRepository, IStyleRepository styleRepository) : IExampleLinksRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext = midjourneyDbContext;
    private readonly IVersionRepository _versionsRepository = versionsRepository;
    private readonly IStyleRepository _stylesRepository = styleRepository;

    // For Commands
    public async Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync
    (
        MidjourneyStyleExampleLink exampleLink,
        CancellationToken cancellationToken
    )
    {
        var versionExist = await _versionsRepository.CheckVersionExistsAsync(exampleLink.Version, cancellationToken);
        if (versionExist.Value is false)
            return Result.Fail<MidjourneyStyleExampleLink>(DomainErrors.VersionNotFound(exampleLink.Version));

        var styleExists = await _stylesRepository.CheckStyleExistsAsync(exampleLink.StyleName, cancellationToken);
        if (styleExists.Value is false)
            return Result.Fail<MidjourneyStyleExampleLink>(DomainErrors.StyleNotFound(exampleLink.StyleName));

        await _midjourneyDbContext.MidjourneyStyleExampleLinks.AddAsync(exampleLink, cancellationToken);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(exampleLink);
    }

    public async Task<Result<BulkDeleteResponse>> DeleteAllExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        var styleExists = await _stylesRepository.CheckStyleExistsAsync(styleName, cancellationToken);
        if (styleExists.Value is false)
            return Result.Fail<BulkDeleteResponse>(DomainErrors.StyleNotFound(styleName));


        var exampleLinks = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .Where(exampleLink => exampleLink.StyleName == styleName)
            .ToListAsync(cancellationToken);

        _midjourneyDbContext.MidjourneyStyleExampleLinks.RemoveRange(exampleLinks);
        var deletedCount = await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(BulkDeleteResponse.Success(deletedCount, $"Exaple links: '{exampleLinks}' was successfully deleted"));
    }

    public async Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(Guid id, CancellationToken cancellationToken)
    {
        var exampleLink = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .FirstOrDefaultAsync(exampleLink => exampleLink.Id == id, cancellationToken);

        if (exampleLink is null) return Result.Fail<MidjourneyStyleExampleLink>(DomainErrors.ExamleLinkNotFound(id));

        _midjourneyDbContext.MidjourneyStyleExampleLinks.Remove(exampleLink);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(exampleLink);
    }

    // For QueriesS
    public async Task<Result<bool>> CheckAnyExampleLinksExistAsync(CancellationToken cancellationToken)
    {
        var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .AnyAsync(cancellationToken);

        return Result.Ok(exist);
    }

    public async Task<Result<bool>> CheckExampleLinkExistsByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .AnyAsync(exampleLink => exampleLink.Id == Id, cancellationToken);

        return Result.Ok(exist);
    }

    public async Task<Result<bool>> CheckExampleLinkExistsByLinkAsync(ExampleLink link, CancellationToken cancellationToken)
    {
        var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .AnyAsync(exampleLink => exampleLink.Link == link, cancellationToken);

        return Result.Ok(exist);
    }

    public async Task<Result<bool>> CheckExampleLinkExistsByStyleAsync(StyleName styleName, CancellationToken cancellationToken)
    {
        var exist = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .AnyAsync(exampleLink => exampleLink.StyleName == styleName, cancellationToken);

        return Result.Ok(exist);
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync(CancellationToken cancellationToken)
    {
        var links = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .Include(exampleLink => exampleLink.MidjuorneyStyle)
            .Include(exampleLink => exampleLink.MidjourneyMaster)
            .ToListAsync(cancellationToken);

        return Result.Ok(links);
    }

    public async Task<Result<MidjourneyStyleExampleLink>> GetExampleLinkByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .Include(exampleLink => exampleLink.MidjuorneyStyle)
            .Include(exampleLink => exampleLink.MidjourneyMaster)
            .FirstOrDefaultAsync(exampleLink => exampleLink.Id == id, cancellationToken);

        if (item is null) return Result.Fail<MidjourneyStyleExampleLink>(DomainErrors.ExamleLinkNotFound(id));
        

        return Result.Ok(item);
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinkByLinkAsync
    (
        ExampleLink link,
        CancellationToken cancellationToken
    )
    {
        var list = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .Include(exampleLink => exampleLink.MidjuorneyStyle)
            .Include(exampleLink => exampleLink.MidjourneyMaster)
            .Where(exampleLink => exampleLink.Link == link)
            .ToListAsync(cancellationToken);

        return Result.Ok(list);
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync
    (
        StyleName styleName,
        CancellationToken cancellationToken
    )
    {
        var list = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .Include(exampleLink => exampleLink.MidjuorneyStyle)
            .Include(exampleLink => exampleLink.MidjourneyMaster)
            .Where(exampleLink => exampleLink.StyleName == styleName)
            .ToListAsync(cancellationToken);

        return Result.Ok(list);
    }

    public async Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync
    (
        StyleName styleName,
        ModelVersion version,
        CancellationToken cancellationToken
    )
    {
        var versionExist = await _versionsRepository.CheckVersionExistsAsync(version, cancellationToken);

        if (versionExist.Value is false)
            return Result.Fail<List<MidjourneyStyleExampleLink>>(DomainErrors.VersionNotFound(version));

        var list = await _midjourneyDbContext.MidjourneyStyleExampleLinks
            .Include(exampleLink => exampleLink.MidjuorneyStyle)
            .Include(exampleLink => exampleLink.MidjourneyMaster)
            .Where(exampleLink => exampleLink.StyleName == styleName && exampleLink.Version == version)
            .ToListAsync(cancellationToken);

        return Result.Ok(list);
    }
}