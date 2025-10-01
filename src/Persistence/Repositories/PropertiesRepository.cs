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

    private static readonly Dictionary<string, PropertiesVersionMapping> _versionMappings = new()
    {
        ["1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1), "MidjourneyPropertiesVersion1"),
        ["2"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2), "MidjourneyPropertiesVersion2"),
        ["3"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3), "MidjourneyPropertiesVersion3"),
        ["4"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4), "MidjourneyPropertiesVersion4"),
        ["5"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5), "MidjourneyPropertiesVersion5"),
        ["5.1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51), "MidjourneyPropertiesVersion51"),
        ["5.2"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52), "MidjourneyPropertiesVersion52"),
        ["6"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6), "MidjourneyPropertiesVersion6"),
        ["6.1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61), "MidjourneyPropertiesVersion61"),
        ["7"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7), "MidjourneyPropertiesVersion7"),
        ["niji 4"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4), "MidjourneyPropertiesVersionNiji4"),
        ["niji 5"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5), "MidjourneyPropertiesVersionNiji5"),
        ["niji 6"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6), "MidjourneyPropertiesVersionNiji6")
    };

    public PropertiesRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    // For Queries
    public Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionAsync(ModelVersion version, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var query = GetQueryableForVersion(version.Value);
            var list = await query.Include(p => p.VersionMaster).ToListAsync(cancellationToken);
            return list;
        }, $"Failed to get parameters for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<bool>> CheckParameterExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var query = GetQueryableForVersion(version.Value);
            return await query.AnyAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);
        }, $"Failed to check parameter existence for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public Task<Result<MidjourneyPropertiesBase>> AddParameterToVersionAsync(MidjourneyPropertiesBase property, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var query = GetQueryableForVersion(property.Version.Value);

            _midjourneyDbContext.Add(property);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return property;
        }, $"Failed to add parameter for version '{property.Version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<MidjourneyPropertiesBase>> UpdateParameterForVersionAsync(MidjourneyPropertiesBase property, CancellationToken cancellationToken)
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

    public async Task<Result<MidjourneyPropertiesBase>> PatchParameterForVersionAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            var query = GetQueryableForVersion(version.Value);
            var parameter = await query.FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);

            UpdateParameterProperty(parameter, characteristicToUpdate, newValue);

            var entry = _midjourneyDbContext.Entry(parameter);
            if (entry.State == EntityState.Detached)
                _midjourneyDbContext.Attach(parameter).State = EntityState.Modified;

            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return parameter;
        }, $"Failed to patch parameter '{propertyName.Value}' for version '{version.Value}'", StatusCodes.Status500InternalServerError);
    }

    public async Task<Result<MidjourneyPropertiesBase>> DeleteParameterInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken)
    {
        var findResult = await ExecuteAsync(async () =>
        {
            var query = GetQueryableForVersion(version.Value);
            return await query.FirstOrDefaultAsync(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value, cancellationToken);
        }, $"Failed to fetch parameter '{propertyName.Value}' for deletion in version '{version.Value}'", StatusCodes.Status500InternalServerError);

        if (findResult.IsFailed)
            return Result.Fail<MidjourneyPropertiesBase>(findResult.Errors);

        var parameter = findResult.Value;
        if (parameter == null)
            return Result.Fail<MidjourneyPropertiesBase>
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

    #region Helpers

    private static void UpdateParameterProperty(MidjourneyPropertiesBase parameter, string propertyToUpdate, string? newValue)
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

    private IQueryable<MidjourneyPropertiesBase> GetQueryableForVersion(string version)
    {
        var mapping = _versionMappings[version];

        var setMethod = typeof(DbContext).GetMethod("Set", [typeof(Type)])
                        ?? throw new InvalidOperationException("DbContext.Set(Type) method not found.");

        var dbSetObj = setMethod.Invoke(_midjourneyDbContext, [mapping.EntityType])
                       ?? throw new InvalidOperationException($"DbSet for type {mapping.EntityType} is null.");

        var queryable = dbSetObj as IQueryable
                        ?? throw new InvalidOperationException($"Returned DbSet is not IQueryable for type {mapping.EntityType}.");

        return queryable.Cast<MidjourneyPropertiesBase>();
    }

    #endregion

    // Version mapping record
    private record PropertiesVersionMapping(Type EntityType, string DbSetPropertyName);
}
