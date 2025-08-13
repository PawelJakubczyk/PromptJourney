using Application.Abstractions;
using Application.Features.Versions;
using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistans.Context;
using System.Reflection;

namespace Persistance.Repositories.Versions;

public sealed class VersionsRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;
    private static string[] _supportedVersions = [];

    // Version mappings dictionary for eliminating switch statements
    private static readonly Dictionary<string, VersionMapping> _versionMappings = new()
    {
        ["1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1), "MidjourneyVersion1"),
        ["2"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2), "MidjourneyVersion2"),
        ["3"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3), "MidjourneyVersion3"),
        ["4"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4), "MidjourneyVersion4"),
        ["5"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5), "MidjourneyVersion5"),
        ["5.1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51), "MidjourneyVersion51"),
        ["5.2"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52), "MidjourneyVersion52"),
        ["6"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6), "MidjourneyVersion6"),
        ["6.1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61), "MidjourneyVersion61"),
        ["7"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7), "MidjourneyVersion7"),
        ["niji 4"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4), "MidjourneyVersionNiji4"),
        ["niji 5"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5), "MidjourneyVersionNiji5"),
        ["niji 6"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6), "MidjourneyVersionNiji6")
    };

    public VersionsRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    public async Task<Result<MidjourneyVersionsMaster>> GetMasterVersionByVersionAsync(string version)
    {
        try
        {
            var versionMaster = await _midjourneyDbContext.MidjourneyVersionsMaster
                .FirstOrDefaultAsync(v => v.Version == version);

            if (versionMaster == null)
                return Result.Fail<MidjourneyVersionsMaster>($"Version '{version}' not found");

            return Result.Ok(versionMaster);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersionsMaster>($"Database error while retrieving version '{version}': {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionMasterAsync(string version)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<List<MidjourneyPropertiesBase>>("Version cannot be null or empty");

            var parameters = await ExecuteVersionOperation(version, async (dbSet) =>
            {
                var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
                return await queryable.ToListAsync();
            });

            return Result.Ok(parameters ?? []);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPropertiesBase>>($"Database error while retrieving version parameters '{version}': {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckVersionExistsInVersionMasterAsync(string version)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<bool>("Version cannot be null or empty");

            if (_supportedVersions.Length == 0)
            {
                _supportedVersions = await _midjourneyDbContext.MidjourneyVersionsMaster
                    .Select(x => x.Version)
                    .ToArrayAsync();
            }

            var exists = _supportedVersions.Contains(version);
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking version existence: {ex.Message}");
        }
    }

    public async Task<Result<ParameterDetails>> AddParameterToVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<ParameterDetails>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                return Result.Fail<ParameterDetails>("Property name cannot be null or empty");

            if (parameters == null || parameters.Length == 0)
                return Result.Fail<ParameterDetails>("Parameters array cannot be null or empty");

            // Check if version exists
            var versionExistsResult = await CheckVersionExistsInVersionMasterAsync(version);
            if (versionExistsResult.IsFailed || !versionExistsResult.Value)
                return Result.Fail<ParameterDetails>($"Version '{version}' does not exist");

            // Check if parameter already exists
            var existsResult = await CheckParameterExistsInVersionAsync(version, propertyName);
            if (existsResult.IsFailed)
                return Result.Fail<ParameterDetails>(existsResult.Errors);

            if (existsResult.Value)
                return Result.Fail<ParameterDetails>($"Parameter '{propertyName}' already exists for version '{version}'");

            // Get VersionMaster
            var versionMaster = await _midjourneyDbContext.MidjourneyVersionsMaster
                .FirstAsync(v => v.Version == version);

            // Create and add parameter using generic method
            var success = await CreateAndAddParameterAsync(version, versionMaster, propertyName, parameters, defaultValue, minValue, maxValue, description);

            if (!success)
                return Result.Fail<ParameterDetails>($"Failed to add parameter to version '{version}'");

            var parameterDetails = new ParameterDetails
            {
                Version = version,
                PropertyName = propertyName,
                Parameters = parameters,
                DefaultValue = defaultValue,
                MinValue = minValue,
                MaxValue = maxValue,
                Description = description
            };

            return Result.Ok(parameterDetails);
        }
        catch (Exception ex)
        {
            return Result.Fail<ParameterDetails>($"Database error while adding parameter: {ex.Message}");
        }
    }

    public async Task<Result<ParameterDetails>> UpdateParameterFromVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<ParameterDetails>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                return Result.Fail<ParameterDetails>("Property name cannot be null or empty");

            if (parameters == null || parameters.Length == 0)
                return Result.Fail<ParameterDetails>("Parameters array cannot be null or empty");

            var versionExistsResult = await CheckVersionExistsInVersionMasterAsync(version);
            if (versionExistsResult.IsFailed || !versionExistsResult.Value)
                return Result.Fail<ParameterDetails>($"Version '{version}' does not exist");

            // Find the parameter using generic method
            var parameter = await FindParameterAsync(version, propertyName);
            if (parameter == null)
                return Result.Fail<ParameterDetails>($"Parameter '{propertyName}' not found for version '{version}'");

            // Update properties
            parameter.Parameters = parameters;
            parameter.DefaultValue = defaultValue;
            parameter.MinValue = minValue;
            parameter.MaxValue = maxValue;
            parameter.Description = description;

            await _midjourneyDbContext.SaveChangesAsync();

            var parameterDetails = new ParameterDetails
            {
                Version = version,
                PropertyName = propertyName,
                Parameters = parameters,
                DefaultValue = defaultValue,
                MinValue = minValue,
                MaxValue = maxValue,
                Description = description
            };

            return Result.Ok(parameterDetails);
        }
        catch (Exception ex)
        {
            return Result.Fail<ParameterDetails>($"Database error while updating parameter: {ex.Message}");
        }
    }

    public async Task<Result<ParameterDetails>> PatchParameterInVersionAsync(string version, string propertyName, string characteristicToUpdate, string? newValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<ParameterDetails>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                return Result.Fail<ParameterDetails>("Property name cannot be null or empty");

            if (string.IsNullOrWhiteSpace(characteristicToUpdate))
                return Result.Fail<ParameterDetails>("Property to update cannot be null or empty");

            var versionExistsResult = await CheckVersionExistsInVersionMasterAsync(version);
            if (versionExistsResult.IsFailed || !versionExistsResult.Value)
                return Result.Fail<ParameterDetails>($"Version '{version}' does not exist");

            var allowedProperties = new[] { "DefaultValue", "MinValue", "MaxValue", "Description", "Parameters" };
            if (!allowedProperties.Contains(characteristicToUpdate, StringComparer.OrdinalIgnoreCase))
                return Result.Fail<ParameterDetails>($"Property '{characteristicToUpdate}' is not supported for patching. Supported properties: {string.Join(", ", allowedProperties)}");

            // Find the parameter
            var parameter = await FindParameterAsync(version, propertyName);
            if (parameter == null)
                return Result.Fail<ParameterDetails>($"Parameter '{propertyName}' not found for version '{version}'");

            // Update specific property
            UpdateParameterProperty(parameter, characteristicToUpdate, newValue);

            await _midjourneyDbContext.SaveChangesAsync();

            var parameterDetails = new ParameterDetails
            {
                Version = parameter.Version,
                PropertyName = parameter.PropertyName,
                Parameters = parameter.Parameters,
                DefaultValue = parameter.DefaultValue,
                MinValue = parameter.MinValue,
                MaxValue = parameter.MaxValue,
                Description = parameter.Description
            };

            return Result.Ok(parameterDetails);
        }
        catch (Exception ex)
        {
            return Result.Fail<ParameterDetails>($"Database error while patching parameter: {ex.Message}");
        }
    }

    public async Task<Result<ParameterDetails>> DeleteParameterInVersionAsync(string version, string propertyName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<ParameterDetails>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                return Result.Fail<ParameterDetails>("Property name cannot be null or empty");

            var versionExistsResult = await CheckVersionExistsInVersionMasterAsync(version);
            if (versionExistsResult.IsFailed || !versionExistsResult.Value)
                return Result.Fail<ParameterDetails>($"Version '{version}' does not exist");

            // Find the parameter
            var parameter = await FindParameterAsync(version, propertyName);
            if (parameter == null)
                return Result.Fail<ParameterDetails>($"Parameter '{propertyName}' not found for version '{version}'");

            // Store details before deletion
            var parameterDetails = new ParameterDetails
            {
                Version = parameter.Version,
                PropertyName = parameter.PropertyName,
                Parameters = parameter.Parameters,
                DefaultValue = parameter.DefaultValue,
                MinValue = parameter.MinValue,
                MaxValue = parameter.MaxValue,
                Description = parameter.Description
            };

            // Remove the parameter
            _midjourneyDbContext.Remove(parameter);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(parameterDetails);
        }
        catch (Exception ex)
        {
            return Result.Fail<ParameterDetails>($"Database error while deleting parameter: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckParameterExistsInVersionAsync(string version, string propertyName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<bool>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                return Result.Fail<bool>("Property name cannot be null or empty");

            var exists = await ExecuteVersionOperation(version, async (dbSet) =>
            {
                var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
                return await queryable.AnyAsync(p => p.PropertyName == propertyName);
            });

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking parameter existence: {ex.Message}");
        }
    }

    private async Task<T?> ExecuteVersionOperation<T>(string version, Func<IQueryable, Task<T>> operation)
    {
        if (!_versionMappings.TryGetValue(version, out var mapping))
            return default;

        var dbSetProperty = typeof(MidjourneyDbContext).GetProperty(mapping.DbSetPropertyName);
        if (dbSetProperty == null)
            return default;

        var dbSet = dbSetProperty.GetValue(_midjourneyDbContext);
        if (dbSet is not IQueryable queryable)
            return default;

        var castMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == "Cast" && m.IsGenericMethodDefinition);
        var genericCastMethod = castMethod.MakeGenericMethod(typeof(MidjourneyPropertiesBase));
        var castedQueryable = (IQueryable)genericCastMethod.Invoke(null, [queryable])!;

        return await operation(castedQueryable);
    }

    private async Task<MidjourneyPropertiesBase?> FindParameterAsync(string version, string propertyName)
    {
        return await ExecuteVersionOperation(version, async (dbSet) =>
        {
            var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
            return await queryable.FirstOrDefaultAsync(p => p.PropertyName == propertyName);
        });
    }

    private async Task<bool> CreateAndAddParameterAsync(string version, MidjourneyVersionsMaster versionMaster, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description)
    {
        if (!_versionMappings.TryGetValue(version, out var mapping))
            return false;

        if (Activator.CreateInstance(mapping.EntityType) is not MidjourneyPropertiesBase instance)
            return false;

        // Set properties
        instance.PropertyName = propertyName;
        instance.Version = version;
        instance.Parameters = parameters;
        instance.DefaultValue = defaultValue;
        instance.MinValue = minValue;
        instance.MaxValue = maxValue;
        instance.Description = description;
        instance.VersionMaster = versionMaster;

        // Get DbSet and add entity
        var dbSetProperty = typeof(MidjourneyDbContext).GetProperty(mapping.DbSetPropertyName);
        if (dbSetProperty?.GetValue(_midjourneyDbContext) is not IQueryable dbSet)
            return false;

        // Use reflection to call Add method
        var addMethod = dbSet.GetType().GetMethod("Add");
        addMethod?.Invoke(dbSet, [instance]);

        await _midjourneyDbContext.SaveChangesAsync();
        return true;
    }

    private static void UpdateParameterProperty(MidjourneyPropertiesBase parameter, string propertyToUpdate, string? newValue)
    {
        switch (propertyToUpdate.ToLowerInvariant())
        {
            case "defaultvalue":
                parameter.DefaultValue = newValue;
                break;
            case "minvalue":
                parameter.MinValue = newValue;
                break;
            case "maxvalue":
                parameter.MaxValue = newValue;
                break;
            case "description":
                parameter.Description = newValue;
                break;
            case "parameters":
                parameter.Parameters = newValue?.Split(',', StringSplitOptions.RemoveEmptyEntries);
                break;
        }
    }

    // Version mapping record
    private record VersionMapping(Type EntityType, string DbSetPropertyName);
}
