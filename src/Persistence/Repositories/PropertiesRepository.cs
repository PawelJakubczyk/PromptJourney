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

public sealed class PropertiesRepository(MidjourneyDbContext midjourneyDbContext, HybridCache cache) : IPropertiesRepository
{
    private const string allPropertiesCacheKey = "all_properties";
    private const string allSupportedPropertiesCacheKey = "all_supported_properties";

    private readonly MidjourneyDbContext _midjourneyDbContext = midjourneyDbContext;
    private readonly HybridCache _cache = cache;

    private readonly HybridCacheEntryOptions cacheOptions = new()
    {
        Expiration = TimeSpan.FromHours(24),
        LocalCacheExpiration = TimeSpan.FromHours(12)
    };

    // For Queries
    public async Task<Result<List<MidjourneyProperties>>> GetAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
            return properties
                .Where(property => property.Version == version)
                .ToList();
        }, $"Failed to get properties for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> GetPropertyByNameAndVersionAsync(PropertyName propertyName, ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
            var property = properties
                .FirstOrDefault(p => p.PropertyName == propertyName && p.Version == version);

            return property ?? throw new KeyNotFoundException($"Property '{propertyName.Value}' not found for version '{version.Value}'");
        }, $"Failed to get property '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<List<MidjourneyProperties>>> GetAllPropertiesAsync(CancellationToken cancellationToken)
    {
        return await ExecuteAsync(
            async () => await GetOrCreateCachedAllPropertiesAsync(cancellationToken),
            "Failed to get all properties",
            StatusCodes.Status500InternalServerError
        );
    }

    public async Task<Result<bool>> CheckPropertyExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
            return properties.Any(p => p.PropertyName == propertyName && p.Version == version);
        }, $"Failed to check property existence for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<List<string>>> GetSupportedPropertyNamesAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
            return properties
                .Where(p => p.Version == version)
                .Select(p => p.PropertyName.Value ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();
        }, $"Failed to get supported property names for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public async Task<Result<MidjourneyProperties>> AddPropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            await _midjourneyDbContext.AddAsync(property, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);

            return property;
        }, $"Failed to add property '{property.PropertyName.Value}' for version '{property.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> UpdatePropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var entry = _midjourneyDbContext.Entry(property);
            if (entry.State == EntityState.Detached)
                _midjourneyDbContext.Attach(property).State = EntityState.Modified;
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);

            return property;
        }, $"Failed to update property '{property.PropertyName.Value}' for version '{property.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> PatchPropertyAsync
    (
        ModelVersion version,
        PropertyName propertyName,
        string characteristicToUpdate,
        string? newValue,
        CancellationToken cancellationToken
    )
    {
        return await ExecuteAsync(async () =>
        {
            var parameter = await _midjourneyDbContext.MidjourneyProperties
                .FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Property '{propertyName.Value}' not found for version '{version.Value}'");

            UpdateParameterProperty(parameter, characteristicToUpdate, newValue);

            var entry = _midjourneyDbContext.Entry(parameter);
            if (entry.State == EntityState.Detached)
                _midjourneyDbContext.Attach(parameter).State = EntityState.Modified;

            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);

            return parameter;
        }, $"Failed to patch property '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> DeletePropertyAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var parameter = await _midjourneyDbContext.MidjourneyProperties
                .FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Property '{propertyName.Value}' not found for version '{version.Value}'");

            _midjourneyDbContext.Remove(parameter);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);

            return parameter;
        }, $"Failed to delete property '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<int>> DeleteAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await _midjourneyDbContext.MidjourneyProperties
                .Where(p => p.Version.Value == version.Value)
                .ToListAsync(cancellationToken);

            if (properties.Count == 0)
                return 0;

            _midjourneyDbContext.RemoveRange(properties);
            var count = await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(cancellationToken);

            return count;
        }, $"Failed to delete all properties for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    // Helper methods for caching
    private async Task<List<MidjourneyProperties>> GetOrCreateCachedAllPropertiesAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            allPropertiesCacheKey,
            async (ct) => await _midjourneyDbContext.MidjourneyProperties.ToListAsync(ct),
            cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task<List<string>> GetOrCreateCachedSupportedPropertiesAsync(CancellationToken cancellationToken) {
        return await _cache.GetOrCreateAsync(
            allSupportedPropertiesCacheKey,
            async (ct) => await _midjourneyDbContext.MidjourneyProperties
                .Select(x => x.PropertyName.Value)
                .Where(x => x != null)
                .Cast<string>()
                .ToListAsync(ct),
            cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(allPropertiesCacheKey, cancellationToken);
        await _cache.RemoveAsync(allSupportedPropertiesCacheKey, cancellationToken);
    }

    // Helpers
    private static void UpdateParameterProperty(MidjourneyProperties parameter, string propertyToUpdate, string? newValue)
    {
        switch (propertyToUpdate.ToLowerInvariant()) {
            case "defaultvalue":
                parameter.DefaultValue = newValue != null ? DefaultValue.Create(newValue).Value : null;
                break;

            case "minvalue":
                parameter.MinValue = newValue != null ? MinValue.Create(newValue).Value : null;
                break;

            case "maxvalue":
                parameter.MaxValue = newValue != null ? MaxValue.Create(newValue).Value : null;
                break;

            case "description":
                parameter.Description = newValue != null ? Description.Create(newValue).Value : null;
                break;

            case "parameters":
                parameter.Parameters = newValue?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => Param.Create(p.Trim()).Value)
                    .Where(p => p != null)
                    .ToList();
                break;

            default:
                throw new ArgumentException($"Unknown property to update: '{propertyToUpdate}'");
        }
    }
}
