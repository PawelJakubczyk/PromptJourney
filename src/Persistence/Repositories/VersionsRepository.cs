using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class VersionsRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _dbContext;

    public VersionsRepository(MidjourneyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // For Queries
    public Task<Result<bool>> CheckVersionExistsInVersionsAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _dbContext.MidjourneyVersionsMaster
                .AnyAsync(v => v.Version == version, cancellationToken);
        }, $"Database error while checking version existence '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckIfAnySupportedVersionExistsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _dbContext.MidjourneyVersionsMaster.AnyAsync(cancellationToken);
        }, "Database error while checking for supported versions", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyVersion>> GetMasterVersionByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        var fetchResult = await ExecuteAsync(async () =>
        {
            return await _dbContext.MidjourneyVersionsMaster
                .FirstOrDefaultAsync(v => v.Version == version, cancellationToken);
        }, $"Database error while retrieving version '{version.Value}'", StatusCodes.Status500InternalServerError);

        return Result.Ok(fetchResult.Value);
    }

    public Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _dbContext.MidjourneyVersionsMaster.ToListAsync(cancellationToken);
        }, "Database error while retrieving all versions", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<ModelVersion>>> GetAllSuportedVersionsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _dbContext.MidjourneyVersionsMaster
                .Select(x => x.Version)
                .ToListAsync(cancellationToken);
        }, "Database error while retrieving supported versions", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion newVersion, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            await _dbContext.MidjourneyVersionsMaster.AddAsync(newVersion, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return newVersion;
        }, "Database error while adding version", StatusCodes.Status500InternalServerError);
    }
}
