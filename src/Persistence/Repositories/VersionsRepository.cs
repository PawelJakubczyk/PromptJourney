using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;

public sealed class VersionsRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _dbContext;

    public VersionsRepository(MidjourneyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // For Queries
    public async Task<Result<bool>> CheckVersionExistsInVersionsAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _dbContext.MidjourneyVersionsMaster
                .AnyAsync(v => v.Version == version, cancellationToken);

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking version existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckIfAnySupportedVersionExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var hasAny = await _dbContext.MidjourneyVersionsMaster
                .AnyAsync(cancellationToken);

            return Result.Ok(hasAny);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking for supported versions: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyVersion>> GetMasterVersionByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        try
        {
            var versionMaster = await _dbContext.MidjourneyVersionsMaster
                .FirstOrDefaultAsync(v => v.Version == version, cancellationToken);

            return versionMaster is null
                ? Result.Fail<MidjourneyVersion>($"Version '{version.Value}' not found")
                : Result.Ok(versionMaster);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersion>($"Database error while retrieving version '{version.Value}': {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var versions = await _dbContext.MidjourneyVersionsMaster
                .ToListAsync(cancellationToken);

            return Result.Ok(versions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyVersion>>($"Database error while retrieving all versions: {ex.Message}");
        }
    }

    public async Task<Result<List<ModelVersion>>> GetAllSuportedVersionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var supportedVersions = await _dbContext.MidjourneyVersionsMaster
                .Select(x => x.Version)
                .ToListAsync(cancellationToken);

            return Result.Ok(supportedVersions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<ModelVersion>>($"Database error while retrieving supported versions: {ex.Message}");
        }
    }

    // For Commands
    public async Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion newVersion, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.MidjourneyVersionsMaster.AddAsync(newVersion, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(newVersion);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersion>($"Database error while adding version: {ex.Message}");
        }
    }
}
