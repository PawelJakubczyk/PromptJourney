//using Application.Abstractions.IRepository;
//using Domain.Entities;
//using Domain.ValueObjects;
//using FluentResults;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Persistence.Context;
//using System;
//using Utilities.Constants;
//using Utilities.Errors;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace Persistence.Repositories2;

//public sealed class PropertiesRepository2 : IPropertiesRepository
//{
//    private readonly MidjourneyDbContext _midjourneyDbContext;

//    private static readonly Dictionary<string, PropertiesVersionMapping> _versionMappings = new()
//    {
//        ["1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1), "MidjourneyPropertiesVersion1"),
//        ["2"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2), "MidjourneyPropertiesVersion2"),
//        ["3"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3), "MidjourneyPropertiesVersion3"),
//        ["4"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4), "MidjourneyPropertiesVersion4"),
//        ["5"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5), "MidjourneyPropertiesVersion5"),
//        ["5.1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51), "MidjourneyPropertiesVersion51"),
//        ["5.2"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52), "MidjourneyPropertiesVersion52"),
//        ["6"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6), "MidjourneyPropertiesVersion6"),
//        ["6.1"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61), "MidjourneyPropertiesVersion61"),
//        ["7"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7), "MidjourneyPropertiesVersion7"),
//        ["niji 4"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4), "MidjourneyPropertiesVersionNiji4"),
//        ["niji 5"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5), "MidjourneyPropertiesVersionNiji5"),
//        ["niji 6"] = new(typeof(MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6), "MidjourneyPropertiesVersionNiji6")
//    };

//    public PropertiesRepository(MidjourneyDbContext midjourneyDbContext)
//    {
//        _midjourneyDbContext = midjourneyDbContext;
//    }

//    // For Queries
//    public async Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionAsync
//    (
//        ModelVersion version,
//        CancellationToken cancellationToken
//    )
//    {
//        try
//        {
//            var parameters = await ExecuteVersionOperation(version.Value, async (dbSet) =>
//            {
//                var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
//                var allParams = await queryable
//                    .Include(p => p.VersionMaster)
//                    .ToListAsync();

//                // Filtruj w pamięci po pobraniu z bazy
//                return allParams.Where(p => p.Version.Value == version.Value).ToList();
//            });

//            return Result.Ok(parameters ?? []);
//        }
//        catch (Exception ex)
//        {
//            var error = new Error<PersistenceLayer>($"Database error while retrieving version parameters '{version.Value}': {ex.Message}", StatusCodes.Status500InternalServerError);
//            return Result.Fail<List<MidjourneyPropertiesBase>>(error);
//        }
//    }

//    public async Task<Result<bool>> CheckParameterExistsInVersionAsync
//    (
//        ModelVersion version, 
//        PropertyName propertyName,
//        CancellationToken cancellationToken
//    )
//    {
//        try
//        {
//            var exists = await ExecuteVersionOperation(version.Value, async (dbSet) =>
//            {
//                var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
                
//                // Pobierz wszystkie parametry do pamięci i sprawdź w pamięci
//                var allParams = await queryable.ToListAsync();
//                return allParams.Any(p => p.PropertyName.Value == propertyName.Value && p.Version.Value == version.Value);
//            });

//            return Result.Ok(exists);
//        }
//        catch (Exception ex)
//        {
//            var error = new Error<PersistenceLayer>($"Database error while checking parameter existence: {ex.Message}", StatusCodes.Status500InternalServerError);
//            return Result.Fail<bool>(error);
//        }
//    }

//    // For Commands
//    public async Task<Result<MidjourneyPropertiesBase>> AddParameterToVersionAsync
//    (
//        MidjourneyPropertiesBase property,
//        CancellationToken cancellationToken
//    )
//    {
//        try
//        {
//            // Check if parameter already exists
//            var existsResult = await CheckParameterExistsInVersionAsync(property.Version, property.PropertyName, cancellationToken);
//            if (existsResult.IsFailed)
//                return Result.Fail<MidjourneyPropertiesBase>(existsResult.Errors);

//            if (existsResult.Value)
//            {
//                var error = new Error<PersistenceLayer>($"Parameter '{property.PropertyName.Value}' already exists for version '{property.Version.Value}'", StatusCodes.Status409Conflict);
//                return Result.Fail<MidjourneyPropertiesBase>(error);

//            }

//            // Get VersionMaster - pobierz wszystkie i znajdź w pamięci
//            var allVersions = await _midjourneyDbContext.MidjourneyVersionsMaster.ToListAsync();
//            var versionMaster = allVersions.FirstOrDefault(v => v.Version.Value == property.Version.Value);

//            if (versionMaster == null)
//            {
//                var error = new Error<PersistenceLayer>($"Version master '{property.Version.Value}' not found", StatusCodes.Status404NotFound);
//                return Result.Fail<MidjourneyPropertiesBase>(error);
//            }

//            // Create and add parameter using generic method
//            var success = await CreateAndAddParameterAsync
//            (
//                property.Version.Value, 
//                versionMaster, 
//                property.PropertyName.Value, 
//                property.Parameters?.Select(p => p.Value).ToArray() ?? [], 
//                property.DefaultValue?.Value, 
//                property.MinValue?.Value, 
//                property.MaxValue?.Value, 
//                property.Description?.Value,
//                cancellationToken
//            );

//            if (!success)
//            {
//                var error = new Error<PersistenceLayer>($"Failed to add parameter to version '{property.Version.Value}'", StatusCodes.Status400BadRequest);
//                return Result.Fail<MidjourneyPropertiesBase>(error);
//            }

//            return Result.Ok(property);
//        }
//        catch (Exception ex)
//        {
//            var error = new Error<PersistenceLayer>($"Database error while adding parameter: {ex.Message}", StatusCodes.Status500InternalServerError);
//            return Result.Fail<MidjourneyPropertiesBase>(error);
//        }
//    }

//    private async Task<T?> ExecuteVersionOperation<T>
//    (
//        string version, 
//        Func<IQueryable, 
//        Task<T>> operation
//    )
//    {
//        if (!_versionMappings.TryGetValue(version, out var mapping))
//            return default;

//        var dbSetProperty = typeof(MidjourneyDbContext).GetProperty(mapping.DbSetPropertyName);
//        if (dbSetProperty == null)
//            return default;

//        var dbSet = dbSetProperty.GetValue(_midjourneyDbContext);
//        if (dbSet is not IQueryable queryable)
//            return default;

//        var castMethod = typeof(Queryable).GetMethods()
//            .First(m => m.Name == "Cast" && m.IsGenericMethodDefinition);
//        var genericCastMethod = castMethod.MakeGenericMethod(typeof(MidjourneyPropertiesBase));
//        var castedQueryable = (IQueryable)genericCastMethod.Invoke(null, [queryable])!;

//        return await operation(castedQueryable);
//    }

//    private async Task<MidjourneyPropertiesBase?> FindParameterAsync
//    (
//        string version,
//        string propertyName,
//        CancellationToken cancellationToken
//    )
//    {
//        return await ExecuteVersionOperation(version, async (dbSet) =>
//        {
//            var queryable = (IQueryable<MidjourneyPropertiesBase>)dbSet;
            
//            var allParams = await queryable.ToListAsync();
//            return allParams.FirstOrDefault(p => p.PropertyName.Value == propertyName && p.Version.Value == version);
//        });
//    }

//    private async Task<bool> CreateAndAddParameterAsync
//    (
//        string version,
//        MidjourneyVersion versionMaster,
//        string propertyName,
//        string[] parameters,
//        string? defaultValue,
//        string? minValue,
//        string? maxValue,
//        string? description,
//        CancellationToken cancellationToken
//    )
//    {
//        if (!_versionMappings.TryGetValue(version, out var mapping))
//            return false;

//        if (Activator.CreateInstance(mapping.EntityType) is not MidjourneyPropertiesBase instance)
//            return false;

//        // Set properties using value objects
//        instance.PropertyName = PropertyName.Create(propertyName).Value;
//        instance.Version = ModelVersion.Create(version).Value;
//        instance.Parameters = parameters?.Select(p => Param.Create(p).Value).ToList();
//        instance.DefaultValue = defaultValue != null ? DefaultValue.Create(defaultValue).Value : null;
//        instance.MinValue = minValue != null ? MinValue.Create(minValue).Value : null;
//        instance.MaxValue = maxValue != null ? MaxValue.Create(maxValue).Value : null;
//        instance.Description = description != null ? Description.Create(description).Value : null;
//        instance.VersionMaster = versionMaster;

//        // Get DbSet and add entity
//        var dbSetProperty = typeof(MidjourneyDbContext).GetProperty(mapping.DbSetPropertyName);
//        if (dbSetProperty?.GetValue(_midjourneyDbContext) is not IQueryable dbSet)
//            return false;

//        // Use reflection to call Add method
//        var addMethod = dbSet.GetType().GetMethod("Add");
//        addMethod?.Invoke(dbSet, [instance]);

//        await _midjourneyDbContext.SaveChangesAsync();
//        return true;
//    }

//    private async Task RemoveParameterAsync
//    (
//        string version,
//        MidjourneyPropertiesBase parameter,
//        CancellationToken cancellationToken
//    )
//    {
//        if (!_versionMappings.TryGetValue(version, out var mapping))
//            throw new InvalidOperationException($"Version '{version}' not supported");

//        var dbSetProperty = typeof(MidjourneyDbContext).GetProperty(mapping.DbSetPropertyName);
//        if (dbSetProperty?.GetValue(_midjourneyDbContext) is not IQueryable dbSet)
//            throw new InvalidOperationException($"DbSet for version '{version}' not found");

//        // Use reflection to call Remove method
//        var removeMethod = dbSet.GetType().GetMethod("Remove");
//        removeMethod?.Invoke(dbSet, [parameter]);

//        await _midjourneyDbContext.SaveChangesAsync();
//    }

//    private static void UpdateParameterProperty
//    (
//        MidjourneyPropertiesBase parameter,
//        string propertyToUpdate,
//        string? newValue,
//        CancellationToken cancellationToken
//    )
//    {
//        switch (propertyToUpdate.ToLowerInvariant())
//        {
//            case "defaultvalue":
//                parameter.DefaultValue = newValue != null ? DefaultValue.Create(newValue).Value : null;
//                break;
//            case "minvalue":
//                parameter.MinValue = newValue != null ? MinValue.Create(newValue).Value : null;
//                break;
//            case "maxvalue":
//                parameter.MaxValue = newValue != null ? MaxValue.Create(newValue).Value : null;
//                break;
//            case "description":
//                parameter.Description = newValue != null ? Description.Create(newValue).Value : null;
//                break;
//            case "parameters":
//                parameter.Parameters = newValue?.Split(',', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(p => Param.Create(p.Trim()).Value).ToList();
//                break;
//        }
//    }

//    // Version mapping record
//    private record PropertiesVersionMapping(Type EntityType, string DbSetPropertyName);
//}
