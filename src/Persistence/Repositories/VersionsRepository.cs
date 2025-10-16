using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Persistence.Context;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class VersionsRepository(MidjourneyDbContext dbContext, HybridCache cache) : IVersionRepository
{
    private const string supportedVersionsCacheKey = "supported_versions";
    private const string allVersionsCacheKey = "all_versions";

    private readonly MidjourneyDbContext _dbContext = dbContext;
    private readonly HybridCache _cache = cache;

    private readonly HybridCacheEntryOptions cacheOptions = new()
    {
        Expiration = TimeSpan.FromDays(365),
        LocalCacheExpiration = TimeSpan.FromDays(365)
    };

    // For Queries
    public Task<Result<bool>> CheckVersionExistsAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var supportedVersions = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
            return supportedVersions.Contains(version.Value);
        }, $"Failed to check if version '{version.Value}' exists", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckIfAnyVersionExistsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var supportedVersions = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
            return supportedVersions.Count > 0;
        }, "Failed to check if any versions exist", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyVersion>> GetVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var allVersions = await GetOrCreateCachedVersionsAsync(cancellationToken);
            var foundVersion = allVersions.FirstOrDefault(v => v.Version.Value == version.Value);

            if (foundVersion == null)
            {
                foundVersion = await _dbContext.MidjourneyVersions
                    .FirstOrDefaultAsync(v => v.Version == version, cancellationToken);

                if (foundVersion == null)
                {
                    throw new KeyNotFoundException($"Version '{version.Value}' not found");
                }

                // Update the cache with the newly found version
                await InvalidateCacheAsync(cancellationToken);
            }

            return foundVersion;
        }, $"Failed to retrieve version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyVersion>> GetLatestVersionAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var allVersions = await GetOrCreateCachedVersionsAsync(cancellationToken);

            if (allVersions.Count == 0)
            {
                throw new InvalidOperationException("No versions found in the database");
            }

            // Get version with most recent release date or first in list if dates are null
            return allVersions
                .OrderByDescending(v => v.ReleaseDate ?? DateTime.MinValue)
                .First();
        }, "Failed to retrieve latest version", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            async () => await GetOrCreateCachedVersionsAsync(cancellationToken),
            "Failed to retrieve all versions",
            StatusCodes.Status500InternalServerError
        );
    }

    public Task<Result<List<string>>> GetAllSupportedVersionsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            async () => await GetOrCreateCachedSupportedVersionsAsync(cancellationToken),
            "Failed to retrieve supported versions",
            StatusCodes.Status500InternalServerError
        );
    }

    // For Commands
    public Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion newVersion, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () => {
            await _dbContext.MidjourneyVersions.AddAsync(newVersion, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);
            return newVersion;
        }, $"Failed to add version '{newVersion.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyVersion>> UpdateVersionAsync(MidjourneyVersion version, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var existingVersion = await _dbContext.MidjourneyVersions
                .FirstOrDefaultAsync(v => v.Version == version.Version, cancellationToken);

            if (existingVersion == null)
            {
                throw new KeyNotFoundException($"Version '{version.Version.Value}' not found");
            }

            _dbContext.Entry(existingVersion).CurrentValues.SetValues(version);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);
            return version;
        }, $"Failed to update version '{version.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyVersion>> DeleteVersionAsync(ModelVersion version, CancellationToken cancellationToken)
        {
        return ExecuteAsync(async () =>
        {
            var existingVersion = await _dbContext.MidjourneyVersions
                .FirstOrDefaultAsync(v => v.Version == version, cancellationToken);

            if (existingVersion == null)
            {
                throw new KeyNotFoundException($"Version '{version.Value}' not found");
            }

            _dbContext.MidjourneyVersions.Remove(existingVersion);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);
            return existingVersion;
        }, $"Failed to delete version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyVersion>> UpdateVersionDescriptionAsync
    (
        ModelVersion version,
        Description description,
        CancellationToken cancellationToken
    )
    {
        return ExecuteAsync(async () =>
        {
            var existingVersion = await _dbContext.MidjourneyVersions
                .FirstOrDefaultAsync(v => v.Version == version, cancellationToken);

            if (existingVersion == null)
            {
                throw new KeyNotFoundException($"Version '{version.Value}' not found");
            }

            existingVersion.Description = description;
            await _dbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);
            return existingVersion;
        }, $"Failed to update description for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    // Helper methods
    private async Task<List<string>> GetOrCreateCachedSupportedVersionsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            supportedVersionsCacheKey,
            async (ct) =>
            {
                var versions = await _dbContext.MidjourneyVersions
                    .Select(Version => Version.Version.Value)
                    .ToListAsync(ct);

                return versions.Where(v => v != null).Cast<string>().ToList();
            },
            cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task<List<MidjourneyVersion>> GetOrCreateCachedVersionsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            allVersionsCacheKey,
            async (ct) =>
            {
                return await _dbContext.MidjourneyVersions
                    .ToListAsync(ct);
            },
            cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(allVersionsCacheKey, cancellationToken);
        await _cache.RemoveAsync(supportedVersionsCacheKey, cancellationToken);
    }
}
