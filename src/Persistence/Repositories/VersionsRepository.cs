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

public sealed class VersionsRepository : IVersionRepository
{
    private const string supportedVersionsCacheKey = "supported_versions";
    private const string allVersionsCacheKey = "all_versions";

    private readonly MidjourneyDbContext _dbContext;
    private readonly HybridCache _cache;

    private readonly HybridCacheEntryOptions cacheOptions = new()
    {
        Expiration = TimeSpan.FromDays(365),
        LocalCacheExpiration = TimeSpan.FromDays(365)
    };

    public VersionsRepository(MidjourneyDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    // For Queries
    public async Task<Result<bool>> CheckVersionExistsAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var supportedVersions = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
            return supportedVersions.Contains(version.Value);
        }, $"Database error while checking version existence '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<bool>> CheckIfAnyVersionExistsAsync(CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var supportedVersions = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
            return supportedVersions.Count > 0;
        }, "Database error while checking for supported versions", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyVersion>> GetVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var allVersions = await GetOrCreateCachedVersionsAsync(cancellationToken);
            var foundVersion = allVersions.FirstOrDefault(v => v.Version.Value == version.Value);

            if (foundVersion == null)
            {
                // If not found in cache, try database as fallback
                foundVersion = await _dbContext.MidjourneyVersions
                    .FirstOrDefaultAsync(v => v.Version == version, cancellationToken);
            }

            return foundVersion!;
        }, $"Database error while retrieving version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            return await GetOrCreateCachedVersionsAsync(cancellationToken);
        }, "Database error while retrieving all versions", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<List<string>>> GetAllSuportedVersionsAsync(CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            return await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
        }, "Database error while retrieving supported versions", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public async Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion newVersion, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            await _dbContext.MidjourneyVersions.AddAsync(newVersion, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await UpdateCachedVersionsAsync(cancellationToken);

            return newVersion;
        }, "Database error while adding version", StatusCodes.Status500InternalServerError);
    }

    // Helper methods
    private async Task<List<string?>> GetOrCreateCachedSupportedVersionsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            supportedVersionsCacheKey,
            async (ct) =>
            {
                return await _dbContext.MidjourneyVersions
                    .Select(x => x.Version.Value)
                    .ToListAsync(ct);
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

    private async Task UpdateCachedVersionsAsync(CancellationToken cancellationToken)
    {
        await _cache.SetAsync(
            allVersionsCacheKey,
            GetOrCreateCachedVersionsAsync(cancellationToken),
            cacheOptions,
            cancellationToken: cancellationToken);

        await _cache.SetAsync(
            supportedVersionsCacheKey,
            GetOrCreateCachedSupportedVersionsAsync(cancellationToken),
            cacheOptions,
            cancellationToken: cancellationToken);
    }
}