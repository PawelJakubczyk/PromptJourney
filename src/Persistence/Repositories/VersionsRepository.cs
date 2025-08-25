using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class VersionsRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;
    private static List<string> _supportedVersions = [];

    public VersionsRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
        _supportedVersions = GetAllSuportedVersionsAsync().Result.Value;

    }

    public async Task<Result<bool>> CheckVersionExistsInVersionsAsync(string version)
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

    public async Task<Result<MidjourneyVersions>> GetMasterVersionByVersionAsync(string version)
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
            return Result.Fail<MidjourneyVersions>($"Database error while retrieving version '{version}': {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyVersions>>> GetAllVersionsAsync()
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
            return Result.Fail<List<MidjourneyVersions>>($"Database error while retrieving all versions: {ex.Message}");
        }
    }

    public async Task<Result<List<string>>> GetAllSuportedVersionsAsync()
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
            return Result.Fail<List<string>>($"Database error while retrieving suported versions: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyVersions>> AddVersionAsync
    (
        string version, 
        string parameter, 
        DateTime? releaseDate = null, 
        string? description = null
    )
    {
        try
        {
            var newVersion = MidjourneyVersions.Create(version, parameter, releaseDate, description).Value;

            await _midjourneyDbContext.MidjourneyVersionsMaster.AddAsync(newVersion);
            await _midjourneyDbContext.SaveChangesAsync();

            _supportedVersions.Add(version);
            return Result.Ok(newVersion);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersions>($"Database error while adding version: {ex.Message}");
        }
    }

    private record VersionMapping(Type EntityType, string DbSetPropertyName);
}
