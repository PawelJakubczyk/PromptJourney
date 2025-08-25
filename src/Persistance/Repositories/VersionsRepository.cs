using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;

namespace Persistance.Repositories;

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
            await Validate.Version.ShouldBeNotNullOrEmpty(version);

            var exists = _supportedVersions.Contains(version);
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking version existence: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyVersions>> GetMasterVersionByVersionAsync(string version)
    {
        try
        {
            await Validate.Version.ShouldBeNotNullOrEmpty(version);

            var versionMaster = await _midjourneyDbContext
                .MidjourneyVersionsMaster
                .FirstOrDefaultAsync(v => v.Version == version);

            if (versionMaster == null)
                return Result.Fail<MidjourneyVersions>($"Version '{version}' not found");

            return Result.Ok(versionMaster);
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

            if (supportedVersions.Count == 0)
            {
                return Result.Fail("No supported version was found.");
            }

            return Result.Ok(supportedVersions);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<string>>($"Database error while retrieving suported versions: {ex.Message}");
        }
    }

    public async Task<Result<MidjourneyVersions>> AddVersionAsync(MidjourneyVersions version)
    {
        try
        {
            await Validate.Version.ShouldBeNotNullOrEmpty(version);
            await Validate.Version.ShouldNotCountainNullOrEmptyProperties(version);

            await _midjourneyDbContext.MidjourneyVersionsMaster.AddAsync(version);
            await _midjourneyDbContext.SaveChangesAsync();
            return Result.Ok(version);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersions>($"Database error while adding version: {ex.Message}");
        }
    }

    private record VersionMapping(Type EntityType, string DbSetPropertyName);
}
