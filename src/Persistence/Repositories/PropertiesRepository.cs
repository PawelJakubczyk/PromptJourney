using Application.Abstractions.IRepository;
using Application.Features.Properties;
using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class PropertiesRepository : IPropertiesRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;
    private static List<string> _supportedVersions = [];

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
        InitializeSupportedVersionsAsync().ConfigureAwait(false);
    }

    private async Task InitializeSupportedVersionsAsync()
    {
        try
        {
            _supportedVersions = await _midjourneyDbContext
                .MidjourneyVersionsMaster
                .Select(x => x.Version)
                .ToListAsync();
        }
        catch
        {
            _supportedVersions = _versionMappings.Keys.ToList();
        }
    }

    // For Queries
    public async Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionAsync(string version)
    {
        try
        {
            //await Validate.Version.Input.CannotBeNullOrEmpty(version);

            //var versionExistsResult = await CheckVersionExistsInVersionsAsync(version);
            //if (versionExistsResult.IsFailed || !versionExistsResult.Value)
            //    return Result.Fail<List<MidjourneyPropertiesBase>>($"Version '{version}' does not exist");

            var parameters = await ExecuteVersionOperation(version, async (dbSet) =>
            {
                var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
                return await queryable
                    .Include(p => p.VersionMaster)
                    .ToListAsync();
            });

            return Result.Ok(parameters ?? []);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPropertiesBase>>($"Database error while retrieving version parameters '{version}': {ex.Message}");
        }
    }

    public async Task<Result<bool>> CheckParameterExistsInVersionAsync(string version, string propertyName)
    {
        try
        {
            //await Validate.Version.Input.CannotBeNullOrEmpty(version);
            //await Validate.Property.Name.Input.MustNotBeNullOrEmpty(propertyName);

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

    // For Commands
    public async Task<Result<PropertyDetails>> AddParameterToVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description)
    {
        try
        {
            // Input validation
            //await Validate.Version.Input.CannotBeNullOrEmpty(version);
            //await Validate.Property.Name.Input.MustNotBeNullOrEmpty(propertyName);
            //await Validate.Property.Parameters.Input.MustNotBeNull(parameters);
            //await Validate.Property.Parameters.Input.MustHaveAtLeastOneElement(parameters);

            //// Check if version exists
            //var versionExistsResult = await CheckVersionExistsInVersionsAsync(version);
            //if (versionExistsResult.IsFailed || !versionExistsResult.Value)
            //    return Result.Fail<PropertyDetails>($"Version '{version}' does not exist");

            // Check if parameter already exists
            var existsResult = await CheckParameterExistsInVersionAsync(version, propertyName);
            if (existsResult.IsFailed)
                return Result.Fail<PropertyDetails>(existsResult.Errors);

            if (existsResult.Value)
                return Result.Fail<PropertyDetails>($"Parameter '{propertyName}' already exists for version '{version}'");

            // Create domain entity first for validation
            var propertyResult = MidjourneyPropertiesBase.Create(propertyName, version, parameters, defaultValue, minValue, maxValue, description);
            if (propertyResult.IsFailed)
                return Result.Fail<PropertyDetails>(propertyResult.Errors);

            // Get VersionMaster
            var versionMaster = await _midjourneyDbContext.MidjourneyVersionsMaster
                .FirstOrDefaultAsync(v => v.Version == version);

            if (versionMaster == null)
                return Result.Fail<PropertyDetails>($"Version master '{version}' not found");

            // Create and add parameter using generic method
            var success = await CreateAndAddParameterAsync(version, versionMaster, propertyName, parameters, defaultValue, minValue, maxValue, description);

            if (!success)
                return Result.Fail<PropertyDetails>($"Failed to add parameter to version '{version}'");

            var parameterDetails = new PropertyDetails
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
            return Result.Fail<PropertyDetails>($"Database error while adding parameter: {ex.Message}");
        }
    }

    public async Task<Result<PropertyDetails>> UpdateParameterForVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description)
    {
        try
        {
            // Input validation
            //await Validate.Version.Input.CannotBeNullOrEmpty(version);
            //await Validate.Property.Name.Input.MustNotBeNullOrEmpty(propertyName);
            //await Validate.Property.Parameters.Input.MustNotBeNull(parameters);
            //await Validate.Property.Parameters.Input.MustHaveAtLeastOneElement(parameters);

            //var versionExistsResult = await CheckVersionExistsInVersionsAsync(version);
            //if (versionExistsResult.IsFailed || !versionExistsResult.Value)
            //    return Result.Fail<PropertyDetails>($"Version '{version}' does not exist");

            // Find the parameter using generic method
            var parameter = await FindParameterAsync(version, propertyName);
            if (parameter == null)
                return Result.Fail<PropertyDetails>($"Parameter '{propertyName}' not found for version '{version}'");

            // Validate new values using domain validation
            var validationResult = MidjourneyPropertiesBase.Create(propertyName, version, parameters, defaultValue, minValue, maxValue, description);
            if (validationResult.IsFailed)
                return Result.Fail<PropertyDetails>(validationResult.Errors);

            // Update properties
            parameter.Parameters = parameters;
            parameter.DefaultValue = defaultValue;
            parameter.MinValue = minValue;
            parameter.MaxValue = maxValue;
            parameter.Description = description;

            await _midjourneyDbContext.SaveChangesAsync();

            var parameterDetails = new PropertyDetails
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
            return Result.Fail<PropertyDetails>($"Database error while updating parameter: {ex.Message}");
        }
    }

    public async Task<Result<PropertyDetails>> PatchParameterForVersionAsync(string version, string propertyName, string characteristicToUpdate, string? newValue)
    {
        try
        {
            // Input validation
            //await Validate.Version.Input.CannotBeNullOrEmpty(version);
            //await Validate.Property.Name.Input.MustNotBeNullOrEmpty(propertyName);

            //var allowedProperties = new[] { "DefaultValue", "MinValue", "MaxValue", "Description", "Parameters" };
            //if (!allowedProperties.Contains(characteristicToUpdate, StringComparer.OrdinalIgnoreCase))
            //    return Result.Fail<PropertyDetails>($"Property '{characteristicToUpdate}' is not supported for patching. Supported properties: {string.Join(", ", allowedProperties)}");

            //var versionExistsResult = await CheckVersionExistsInVersionsAsync(version);
            //if (versionExistsResult.IsFailed || !versionExistsResult.Value)
            //    return Result.Fail<PropertyDetails>($"Version '{version}' does not exist");

            // Find the parameter
            var parameter = await FindParameterAsync(version, propertyName);
            if (parameter == null)
                return Result.Fail<PropertyDetails>($"Parameter '{propertyName}' not found for version '{version}'");

            // Update specific property
            UpdateParameterProperty(parameter, characteristicToUpdate, newValue);

            // Validate after update
            var validationResult = MidjourneyPropertiesBase.Create(
                parameter.PropertyName,
                parameter.Version,
                parameter.Parameters,
                parameter.DefaultValue,
                parameter.MinValue,
                parameter.MaxValue,
                parameter.Description);

            if (validationResult.IsFailed)
                return Result.Fail<PropertyDetails>(validationResult.Errors);

            await _midjourneyDbContext.SaveChangesAsync();

            var parameterDetails = new PropertyDetails
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
            return Result.Fail<PropertyDetails>($"Database error while patching parameter: {ex.Message}");
        }
    }

    public async Task<Result<PropertyDetails>> DeleteParameterInVersionAsync(string version, string propertyName)
    {
        try
        {
            // Input validation
            //await Validate.Version.Input.CannotBeNullOrEmpty(version);
            //await Validate.Property.Name.Input.MustNotBeNullOrEmpty(propertyName);

            //var versionExistsResult = await CheckVersionExistsInVersionsAsync(version);
            //if (versionExistsResult.IsFailed || !versionExistsResult.Value)
            //    return Result.Fail<PropertyDetails>($"Version '{version}' does not exist");

            // Find the parameter
            var parameter = await FindParameterAsync(version, propertyName);
            if (parameter == null)
                return Result.Fail<PropertyDetails>($"Parameter '{propertyName}' not found for version '{version}'");

            // Store details before deletion
            var parameterDetails = new PropertyDetails
            {
                Version = parameter.Version,
                PropertyName = parameter.PropertyName,
                Parameters = parameter.Parameters,
                DefaultValue = parameter.DefaultValue,
                MinValue = parameter.MinValue,
                MaxValue = parameter.MaxValue,
                Description = parameter.Description
            };

            // Remove the parameter using generic method
            await RemoveParameterAsync(version, parameter);

            return Result.Ok(parameterDetails);
        }
        catch (Exception ex)
        {
            return Result.Fail<PropertyDetails>($"Database error while deleting parameter: {ex.Message}");
        }
    }

    private async Task<bool> CheckVersionExistsInVersionsAsync(string version)
    {
        try
        {
            if (_supportedVersions.Count == 0)
                await InitializeSupportedVersionsAsync();

            return await _midjourneyDbContext.MidjourneyVersionsMaster
                .AnyAsync(v => v.Version == version);
        }
        catch
        {
            return false;
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

    private async Task<bool> CreateAndAddParameterAsync(string version, MidjourneyVersions versionMaster, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description)
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

    private async Task RemoveParameterAsync(string version, MidjourneyPropertiesBase parameter)
    {
        if (!_versionMappings.TryGetValue(version, out var mapping))
            throw new InvalidOperationException($"Version '{version}' not supported");

        var dbSetProperty = typeof(MidjourneyDbContext).GetProperty(mapping.DbSetPropertyName);
        if (dbSetProperty?.GetValue(_midjourneyDbContext) is not IQueryable dbSet)
            throw new InvalidOperationException($"DbSet for version '{version}' not found");

        // Use reflection to call Remove method
        var removeMethod = dbSet.GetType().GetMethod("Remove");
        removeMethod?.Invoke(dbSet, [parameter]);

        await _midjourneyDbContext.SaveChangesAsync();
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
    private record PropertiesVersionMapping(Type EntityType, string DbSetPropertyName);
}
