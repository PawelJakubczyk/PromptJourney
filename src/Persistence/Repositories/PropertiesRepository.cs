using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Persistence.Context;
using Utilities.Constants;
using Utilities.Extensions;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class PropertiesRepository : IPropertiesRepository
{
    const string allPropertiesCacheKey = "all_properties";
    const string allSuportedPropertiesCacheKey = "all_suported_properties";

    private readonly MidjourneyDbContext _midjourneyDbContext;
    private readonly HybridCache _cache;

    private readonly HybridCacheEntryOptions cacheOptions = new()
    {
        Expiration = TimeSpan.FromHours(24),
        LocalCacheExpiration = TimeSpan.FromHours(12)
    };

    public PropertiesRepository(MidjourneyDbContext midjourneyDbContext, HybridCache cache)
    {
        _midjourneyDbContext = midjourneyDbContext;
        _cache = cache;
    }

    // For Queries
    public async Task<Result<List<MidjourneyProperties>>> GetAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
            return properties
                .Where(property => property.Version == version)
                .ToList();
        }, $"Failed to get Properties for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<List<MidjourneyProperties>>> GetAllProperties(CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () => await GetOrCreateCachedAllPropertiesAsync(cancellationToken),
        $"Failed to get all Properties", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<bool>> CheckPropertyExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var properties = await GetOrCreateCachedSuportedPropertiesAsync(cancellationToken);
            return properties.Contains(propertyName.Value);
        }, $"Failed to check Properties existence for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public async Task<Result<MidjourneyProperties>> AddProperyAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            _midjourneyDbContext.Add(property);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await UpdateCachedPropertyAsync(cancellationToken);

            return property;
        }, $"Failed to add Properties for version '{property.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> UpdatePropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var entry = _midjourneyDbContext.Entry(property);
            if (entry.State == EntityState.Detached)
                _midjourneyDbContext.Attach(property).State = EntityState.Modified;
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await UpdateCachedPropertyAsync(cancellationToken);

            return property;
        }, $"Failed to update Property", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> PatchPropertyAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var query = _midjourneyDbContext.MidjourneyProperties;
            var parameter = await query.FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);

            UpdateParameterProperty(parameter, characteristicToUpdate, newValue);

            var entry = _midjourneyDbContext.Entry(parameter);
            if (entry.State == EntityState.Detached)
                _midjourneyDbContext.Attach(parameter).State = EntityState.Modified;

            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await UpdateCachedPropertyAsync(cancellationToken);

            return parameter;
        }, $"Failed to patch Property'{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> DeletePropertyAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        var findResult = await ExecuteAsync(async () =>
        {
            var query = _midjourneyDbContext.MidjourneyProperties;
            return await query.FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);
        }, $"Failed to fetch Property '{propertyName.Value}' for deletion in version '{version.Value}'", StatusCodes.Status500InternalServerError);

        if (findResult.IsFailed)
            return Result.Fail<MidjourneyProperties>(findResult.Errors);

        var parameter = findResult.Value;
        if (parameter == null)
            return Result.Fail<MidjourneyProperties>
                (
                    ErrorFactory.Create()
                    .Withlayer(typeof(PersistenceLayer))
                    .WithMessage($"Property '{propertyName.Value}' not found in version '{version.Value}'")
                    .WithErrorCode(StatusCodes.Status404NotFound)
                );

        var deleteResult = await ExecuteAsync(async () =>
        {
            _midjourneyDbContext.Remove(parameter);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            await UpdateCachedPropertyAsync(cancellationToken);

            return parameter;
        }, $"Failed to delete Property '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);

        return deleteResult;
    }

    // Helper methods for caching
    private async Task<List<MidjourneyProperties>> GetOrCreateCachedAllPropertiesAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            allPropertiesCacheKey,
            async (ct) =>
            {
                return await _midjourneyDbContext.MidjourneyProperties.ToListAsync(ct);
            },
            cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task<List<string?>> GetOrCreateCachedSuportedPropertiesAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            allSuportedPropertiesCacheKey,
            async (ct) =>
            {
                return await _midjourneyDbContext.MidjourneyProperties
                    .Select(x => x.PropertyName.Value)
                    .ToListAsync(ct);
            },
            cacheOptions,
            cancellationToken: cancellationToken
        );
    }

    private async Task UpdateCachedPropertyAsync(CancellationToken cancellationToken)
    {
        await _cache.SetAsync(
            allPropertiesCacheKey,
            GetOrCreateCachedAllPropertiesAsync(cancellationToken),
            cacheOptions,
            cancellationToken: cancellationToken);

        await _cache.SetAsync(
            allSuportedPropertiesCacheKey,
            GetOrCreateCachedSuportedPropertiesAsync(cancellationToken),
            cacheOptions,
            cancellationToken: cancellationToken);
    }

    // Helpers
    private static void UpdateParameterProperty(MidjourneyProperties parameter, string propertyToUpdate, string? newValue)
    {
        switch (propertyToUpdate.ToLowerInvariant())
        {
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
                    .Select(p => Param.Create(p.Trim()).Value).ToList();
                break;
            default:
                // ignore unknown properties
                break;
        }
    }
}