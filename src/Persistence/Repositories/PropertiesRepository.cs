using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Utilities.Constants;
using Utilities.Extensions;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class PropertiesRepository : IPropertiesRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public PropertiesRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    // For Queries
    public Task<Result<List<MidjourneyProperties>>> GetAllParametersByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var query = _midjourneyDbContext.MidjourneyProperties;
            var list = await query.Include(p => p.VersionMaster).ToListAsync(cancellationToken);
            return list;
        }, $"Failed to get parameters for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckParameterExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var query = _midjourneyDbContext.MidjourneyProperties;
            return await query.AnyAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);
        }, $"Failed to check parameter existence for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public Task<Result<MidjourneyProperties>> AddParameterToVersionAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var query = _midjourneyDbContext.MidjourneyProperties;

            _midjourneyDbContext.Add(property);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return property;
        }, $"Failed to add parameter for version '{property.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyProperties>> UpdateParameterForVersionAsync(MidjourneyProperties property, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var entry = _midjourneyDbContext.Entry(property);
            if (entry.State == EntityState.Detached)
                _midjourneyDbContext.Attach(property).State = EntityState.Modified;

            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return property;
        }, $"Failed to update parameter for version '{property.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> PatchParameterForVersionAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken)
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
            return parameter;
        }, $"Failed to patch parameter '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyProperties>> DeleteParameterInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        var findResult = await ExecuteAsync(async () =>
        {
            var query = _midjourneyDbContext.MidjourneyProperties;
            return await query.FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);
        }, $"Failed to fetch parameter '{propertyName.Value}' for deletion in version '{version.Value}'", StatusCodes.Status500InternalServerError);

        if (findResult.IsFailed)
            return Result.Fail<MidjourneyProperties>(findResult.Errors);

        var parameter = findResult.Value;
        if (parameter == null)
            return Result.Fail<MidjourneyProperties>
                (
                    ErrorFactory.Create()
                    .Withlayer(typeof(PersistenceLayer))
                    .WithMessage($"Parameter '{propertyName.Value}' not found in version '{version.Value}'")
                    .WithErrorCode(StatusCodes.Status404NotFound)
                );



        var deleteResult = await ExecuteAsync(async () =>
        {
            _midjourneyDbContext.Remove(parameter);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return parameter;
        }, $"Failed to delete parameter '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);

        return deleteResult;
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
