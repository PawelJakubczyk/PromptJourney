using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class VersionsRepository(MidjourneyDbContext dbContext, HybridCache cache) : IVersionRepository
{
    private const string _supportedVersionsCacheKey = "supported_versions";
    private const string _allVersionsCacheKey = "all_versions";

    private readonly MidjourneyDbContext _dbContext = dbContext;
    private readonly HybridCache _cache = cache;

    private readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        Expiration = TimeSpan.FromDays(365),
        LocalCacheExpiration = TimeSpan.FromDays(365)
    };

    public async Task<Result<bool>> CheckVersionExistsAsync(ModelVersion modelVersion, CancellationToken cancellationToken)
    {
        var supportedVersions = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
        var exists = supportedVersions.Contains(modelVersion.Value);
        return Result.Ok(exists);
    }

    public async Task<Result<bool>> CheckIfAnyVersionExistsAsync(CancellationToken cancellationToken)
    {
        var supportedVersions = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
        return Result.Ok(supportedVersions.Count > 0);
    }

    public async Task<Result<MidjourneyVersion>> GetVersionAsync(ModelVersion modelVersion, CancellationToken cancellationToken)
    {
        var allVersions = await GetOrCreateCachedVersionsAsync(cancellationToken);
        var foundVersion = allVersions.FirstOrDefault(version => version.Version.Value == modelVersion.Value);

        if (foundVersion is null) 
            return Result.Fail<MidjourneyVersion>(DomainErrors.VersionNotFound(modelVersion));

        return Result.Ok(foundVersion);
    }

    public async Task<Result<MidjourneyVersion>> GetLatestVersionAsync(CancellationToken cancellationToken)
    {
        var allVersions = await GetOrCreateCachedVersionsAsync(cancellationToken);

        if (allVersions.Count is 0) 
            return Result.Fail<MidjourneyVersion>(DomainErrors.NoVersionFound());

        var latest = allVersions
            .OrderByDescending(version => version.ReleaseDate ?? DateTime.MinValue)
            .First();

        return Result.Ok(latest);
    }

    public async Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken)
    {
        var versions = await GetOrCreateCachedVersionsAsync(cancellationToken);
        return Result.Ok(versions);
    }

    public async Task<Result<List<string>>> GetAllSupportedVersionsAsync(CancellationToken cancellationToken)
    {
        var supported = await GetOrCreateCachedSupportedVersionsAsync(cancellationToken);
        return Result.Ok(supported);
    }

    // For Commands
    public async Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion newVersion, CancellationToken cancellationToken)
    {
        await _dbContext.MidjourneyVersions.AddAsync(newVersion, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);
        return Result.Ok(newVersion);
    }

    // Helper methods
    private async Task<List<string?>> GetOrCreateCachedSupportedVersionsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync
        (
            _supportedVersionsCacheKey,
            async (ct) =>
            {
                var versions = await _dbContext.MidjourneyVersions
                    .Select(Version => Version.Version.Value)
                    .ToListAsync(ct) ?? [];

                return versions;
            },
            _cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task<List<MidjourneyVersion>> GetOrCreateCachedVersionsAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync
        (
            _allVersionsCacheKey,
            async (ct) =>
            {
                var versions = await _dbContext.MidjourneyVersions
                    .ToListAsync(ct);

                return versions;
            },
            _cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(_allVersionsCacheKey, cancellationToken);
        await _cache.RemoveAsync(_supportedVersionsCacheKey, cancellationToken);
    }
}
