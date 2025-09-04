using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Errors;

namespace Persistence.Repositories;

public sealed class VersionsRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;
    private static List<ModelVersion> _supportedVersions = [];

    public VersionsRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
        _supportedVersions = GetAllSuportedVersionsAsync().Result.Value;

    }

    public async Task<Result<bool>> CheckVersionExistsInVersionsAsync(ModelVersion version)
    {
        try
        {
            var exists = await Task.Run(() => _supportedVersions.Contains(version));
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking version existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckIfAnySupportedVersionExistsAsync()
    {
        try
        {
            var hasAny = await _midjourneyDbContext
                .MidjourneyVersionsMaster
                .AnyAsync();

            return Result.Ok(hasAny);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>( $"Database error while checking for supported versions: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyVersion>> GetMasterVersionByVersionAsync(ModelVersion version)
    {
        try
        {
            var versionMaster = await _midjourneyDbContext
                .MidjourneyVersionsMaster
                .FirstOrDefaultAsync(v => v.Version == version);

            return Result.Ok(versionMaster!);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersion>($"Database error while retrieving version '{version}': {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync()
    {
        try
        {
            var versions = await _midjourneyDbContext
                .MidjourneyVersionsMaster
                .ToListAsync();
            return Result.Ok(versions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyVersion>>($"Database error while retrieving all versions: {ex.Message}");
        }
    }

    public async Task<Result<List<ModelVersion>>> GetAllSuportedVersionsAsync()
    {
        try
        {
            var supportedVersions = await _midjourneyDbContext
                .MidjourneyVersionsMaster
                .Select(x => x.Version)
                .ToListAsync();

            return Result.Ok(supportedVersions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<ModelVersion>>($"Database error while retrieving suported versions: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion newVersion)
    {
        try
        {
            await _midjourneyDbContext.MidjourneyVersionsMaster.AddAsync(newVersion);
            await _midjourneyDbContext.SaveChangesAsync();

            _supportedVersions.Add(newVersion.Version);
            return Result.Ok(newVersion);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersion>($"Database error while adding version: {ex.Message}");
        }
    }
}