using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Persistence.Context;
using Utilities.Constants;
using Utilities.Errors;

namespace Persistence.Repositories;

public sealed class PropertiesRepository(MidjourneyDbContext midjourneyDbContext, HybridCache cache) : IPropertiesRepository
{
    private const string allPropertiesCacheKey = "all_properties";
    private const string allSupportedPropertiesCacheKey = "all_supported_properties";

    private readonly MidjourneyDbContext _midjourneyDbContext = midjourneyDbContext;
    private readonly HybridCache _cache = cache;

    private readonly HybridCacheEntryOptions cacheOptions = new()
    {
        Expiration = TimeSpan.FromDays(365),
        LocalCacheExpiration = TimeSpan.FromDays(365)
    };

    // For Queries
    public async Task<Result<List<MidjourneyProperties>>> GetAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
        var propertiesByVersion = properties
            .Where(property => property.Version == version)
            .ToList();

        return Result.Ok(propertiesByVersion);
    }

    public async Task<Result<MidjourneyProperties>> GetPropertyByNameAndVersionAsync(PropertyName propertyName, ModelVersion version, CancellationToken cancellationToken)
    {
        var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
        var property = properties
            .FirstOrDefault(p => p.PropertyName == propertyName && p.Version == version);

        if (property is null) return Result.Fail<MidjourneyProperties>(DomainErrors.PropertyNotFound(propertyName));

        return Result.Ok(property);
    }

    public async Task<Result<List<MidjourneyProperties>>> GetAllPropertiesAsync(CancellationToken cancellationToken)
    {
        var properties = await GetOrCreateCachedAllPropertiesAsync(cancellationToken);
        return Result.Ok(properties);
    }

    public async Task<Result<bool>> CheckPropertyExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        var properties = await GetOrCreateCachedSupportedPropertiesAsync(cancellationToken);
        var exist = properties.Any(property => property == $"{propertyName.Value}:{version.Value}");
        return Result.Ok(exist);
    }

    // For Commands
    public async Task<Result<MidjourneyProperties>> AddPropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        await _midjourneyDbContext.AddAsync(property, cancellationToken);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);

        return Result.Ok(property);
    }

    public async Task<Result<MidjourneyProperties>> UpdatePropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        var entry = _midjourneyDbContext.Entry(property);
        if (entry.State == EntityState.Detached)
            _midjourneyDbContext.Attach(property).State = EntityState.Modified;

        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);

        return Result.Ok(property);
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
        var parameter = await _midjourneyDbContext.MidjourneyProperties
            .FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);

        if (parameter == null)
        {
            var notFoundError = ErrorBuilder.New()
                .WithLayer<DomainLayer>()
                .WithMessage($"Property '{propertyName.Value}' not found for version '{version.Value}'")
                .WithErrorCode(StatusCodes.Status404NotFound)
                .Build();

            return Result.Fail<MidjourneyProperties>(notFoundError);
        }

        UpdateParameterProperty(parameter, characteristicToUpdate, newValue);

        var entry = _midjourneyDbContext.Entry(parameter);
        if (entry.State == EntityState.Detached)
            _midjourneyDbContext.Attach(parameter).State = EntityState.Modified;

        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);

        return Result.Ok(parameter);
    }

    public async Task<Result<MidjourneyProperties>> DeletePropertyAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        var parameter = await _midjourneyDbContext.MidjourneyProperties
            .FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);

        if (parameter == null)
        {
            var notFoundError = ErrorBuilder.New()
                .WithLayer<DomainLayer>()
                .WithMessage($"Property '{propertyName.Value}' not found for version '{version.Value}'")
                .WithErrorCode(StatusCodes.Status404NotFound)
                .Build();

            return Result.Fail<MidjourneyProperties>(notFoundError);
        }

        _midjourneyDbContext.Remove(parameter);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);

        return Result.Ok(parameter);
    }

    public async Task<Result<int>> DeleteAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        var properties = await _midjourneyDbContext.MidjourneyProperties
            .Where(p => p.Version.Value == version.Value)
            .ToListAsync(cancellationToken);

        if (properties.Count == 0)
            return Result.Ok(0);

        _midjourneyDbContext.RemoveRange(properties);
        var count = await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(cancellationToken);

        return Result.Ok(count);
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

    private async Task<List<string>> GetOrCreateCachedSupportedPropertiesAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            allSupportedPropertiesCacheKey,
            async ct =>
            {
                return await _midjourneyDbContext.MidjourneyProperties
                    .Where(p => p.PropertyName != null && p.Version != null)
                    .Select(p => $"{p.PropertyName.Value}:{p.Version.Value}")
                    .ToListAsync(ct);
            },
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
