using Application.Abstractions;
using Application.Features.Versions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistans.Context;

namespace Persistance.Repositories;

public sealed class VersionRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public VersionRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    public async Task<Result<MidjourneyVersionsMaster>> GetMasterVersionByVersionAsync(string version)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<MidjourneyVersionsMaster>("Version cannot be null or empty");

            var versionMasterQuery = _midjourneyDbContext
                                    .MidjourneyVersionsMaster
                                    .Where(v => v.Version == version)
                                    .AsQueryable();

            var versionMaster = await versionMasterQuery
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

    private static string[] SupportedVersions = [];

    public async Task<Result<bool>> CheckVersionExistsInVersionMasterAsync(string version)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(version))
                return Result.Fail<bool>("Version cannot be null or empty");

            if (SupportedVersions is null || SupportedVersions.Length == 0)
            {
                SupportedVersions = await _midjourneyDbContext.MidjourneyVersionsMaster.Select(x => x.Version).ToArrayAsync();
            }

            var exists = SupportedVersions.Contains(version);

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
                Result.Fail<ParameterDetails>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                Result.Fail<ParameterDetails>("Property name cannot be null or empty");

            if (parameters == null || parameters.Length == 0)
                Result.Fail<ParameterDetails>("Parameters array cannot be null or empty");

            // Check if version exists
            var versionResult = await GetAllParametersByVersionMasterAsync(version);
            if (versionResult.IsFailed)
                Result.Fail<ParameterDetails>(versionResult.Errors);

            // Check if parameter already exists
            var existsResult = await CheckParameterExistsInVersionAsync(version, propertyName);
            if (existsResult.IsFailed)
                Result.Fail<ParameterDetails>(existsResult.Errors);

            if (existsResult.Value)
                Result.Fail<ParameterDetails>($"Parameter '{propertyName}' already exists for version '{version}'");

            var versionMaster = versionResult.Value;

            var parameter = new MidjourneyVersionsBase
            {
                Version = version,
                PropertyName = propertyName,
                Parameters = parameters,
                DefaultValue = defaultValue,
                MinValue = minValue,
                MaxValue = maxValue,
                Description = description,
                VersionMaster = new()
                {
                    Version = version
                }
                //VersionMaster = versionMaster.First()
            };

            bool success = version switch
            {
                "1" => _midjourneyDbContext.MidjourneyVersion1.Add((MidjourneyAllVersions.MidjourneyVersion1)parameter) != null,
                "2" => _midjourneyDbContext.MidjourneyVersion2.Add((MidjourneyAllVersions.MidjourneyVersion2)parameter) != null,
                "3" => _midjourneyDbContext.MidjourneyVersion3.Add((MidjourneyAllVersions.MidjourneyVersion3)parameter) != null,
                "4" => _midjourneyDbContext.MidjourneyVersion4.Add((MidjourneyAllVersions.MidjourneyVersion4)parameter) != null,
                "5" => _midjourneyDbContext.MidjourneyVersion5.Add((MidjourneyAllVersions.MidjourneyVersion5)parameter) != null,
                "5.1" => _midjourneyDbContext.MidjourneyVersion51.Add((MidjourneyAllVersions.MidjourneyVersion51)parameter) != null,
                "5.2" => _midjourneyDbContext.MidjourneyVersion52.Add((MidjourneyAllVersions.MidjourneyVersion52)parameter) != null,
                "6" => _midjourneyDbContext.MidjourneyVersion6.Add((MidjourneyAllVersions.MidjourneyVersion6)parameter) != null,
                "6.1" => _midjourneyDbContext.MidjourneyVersion61.Add((MidjourneyAllVersions.MidjourneyVersion61)parameter) != null,
                "7" => _midjourneyDbContext.MidjourneyVersion7.Add((MidjourneyAllVersions.MidjourneyVersion7)parameter) != null,
                "niji4" => _midjourneyDbContext.MidjourneyVersionNiji4.Add((MidjourneyAllVersions.MidjourneyVersionNiji4)parameter) != null,
                "niji5" => _midjourneyDbContext.MidjourneyVersionNiji5.Add((MidjourneyAllVersions.MidjourneyVersionNiji5)parameter) != null,
                "niji6" => _midjourneyDbContext.MidjourneyVersionNiji6.Add((MidjourneyAllVersions.MidjourneyVersionNiji6)parameter) != null,
                _ => false
            };

            await _midjourneyDbContext.SaveChangesAsync();


            if (!success)
                return Result.Fail<ParameterDetails>($"Unsupported version '{version}' or failed to add parameter");

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
            {
                return Result.Fail<ParameterDetails>($"Version '{version}' does not exist");
            }

            // Find the parameter
            MidjourneyVersionsBase? parameter = version switch
            {
                "1" => await _midjourneyDbContext.MidjourneyVersion1.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "2" => await _midjourneyDbContext.MidjourneyVersion2.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "3" => await _midjourneyDbContext.MidjourneyVersion3.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "4" => await _midjourneyDbContext.MidjourneyVersion4.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5" => await _midjourneyDbContext.MidjourneyVersion5.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5.1" => await _midjourneyDbContext.MidjourneyVersion51.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5.2" => await _midjourneyDbContext.MidjourneyVersion52.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "6" => await _midjourneyDbContext.MidjourneyVersion6.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "6.1" => await _midjourneyDbContext.MidjourneyVersion61.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "7" => await _midjourneyDbContext.MidjourneyVersion7.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 4" => await _midjourneyDbContext.MidjourneyVersionNiji4.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 5" => await _midjourneyDbContext.MidjourneyVersionNiji5.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 6" => await _midjourneyDbContext.MidjourneyVersionNiji6.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                _ => null
            };

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
            {
                return Result.Fail<ParameterDetails>($"Version '{version}' does not exist");
            }

            var allowedProperties = new[] { "DefaultValue", "MinValue", "MaxValue", "Description", "Parameters" };
            if (!allowedProperties.Contains(characteristicToUpdate))
            {
                return Result.Fail<ParameterDetails>($"Property '{characteristicToUpdate}' is not supported for patching. Supported properties: DefaultValue, MinValue, MaxValue, Description, Parameters");
            }

            // Find the parameter
            MidjourneyVersionsBase? parameter = version switch
            {
                "1" => await _midjourneyDbContext.MidjourneyVersion1.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "2" => await _midjourneyDbContext.MidjourneyVersion2.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "3" => await _midjourneyDbContext.MidjourneyVersion3.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "4" => await _midjourneyDbContext.MidjourneyVersion4.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5" => await _midjourneyDbContext.MidjourneyVersion5.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5.1" => await _midjourneyDbContext.MidjourneyVersion51.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5.2" => await _midjourneyDbContext.MidjourneyVersion52.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "6" => await _midjourneyDbContext.MidjourneyVersion6.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "6.1" => await _midjourneyDbContext.MidjourneyVersion61.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "7" => await _midjourneyDbContext.MidjourneyVersion7.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 4" => await _midjourneyDbContext.MidjourneyVersionNiji4.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 5" => await _midjourneyDbContext.MidjourneyVersionNiji5.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 6" => await _midjourneyDbContext.MidjourneyVersionNiji6.FirstOrDefaultAsync(p => p.PropertyName == propertyName)
            };

            // Update specific property
            switch (characteristicToUpdate.ToLowerInvariant())
            {
                case "defaultvalue":
                    parameter!.DefaultValue = newValue;
                    break;
                case "minvalue":
                    parameter!.MinValue = newValue;
                    break;
                case "maxvalue":
                    parameter!.MaxValue = newValue;
                    break;
                case "description":
                    parameter!.Description = newValue;
                    break;
                case "parameters":
                    if (newValue != null)
                    {
                        parameter!.Parameters = newValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    }
                    break;
            }

            object? updated = characteristicToUpdate.ToLowerInvariant() switch
            {
                "defaultvalue" => parameter!.DefaultValue = newValue,
                "minvalue" => parameter!.MinValue = newValue,
                "maxvalue" => parameter!.MaxValue = newValue,
                "description" => parameter!.Description = newValue,
                "parameters" => parameter!.Parameters = newValue?.Split(',', StringSplitOptions.RemoveEmptyEntries),
                _ => null
            };


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
            var versionExistsResult = await CheckVersionExistsInVersionMasterAsync(version);

            if (versionExistsResult.IsFailed || !versionExistsResult.Value)
            {
                Result.Fail<ParameterDetails>($"Version '{version}' does not exist");
            }

            if (string.IsNullOrWhiteSpace(version))
                Result.Fail<ParameterDetails>("Version cannot be null or empty");

            if (string.IsNullOrWhiteSpace(propertyName))
                Result.Fail<ParameterDetails>("Property name cannot be null or empty");

            //// Find the parameter first to get its details
            MidjourneyVersionsBase? parameter = version switch
            {
                "1" => await _midjourneyDbContext.MidjourneyVersion1.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "2" => await _midjourneyDbContext.MidjourneyVersion2.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "3" => await _midjourneyDbContext.MidjourneyVersion3.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "4" => await _midjourneyDbContext.MidjourneyVersion4.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5" => await _midjourneyDbContext.MidjourneyVersion5.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5.1" => await _midjourneyDbContext.MidjourneyVersion51.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "5.2" => await _midjourneyDbContext.MidjourneyVersion52.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "6" => await _midjourneyDbContext.MidjourneyVersion6.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "6.1" => await _midjourneyDbContext.MidjourneyVersion61.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "7" => await _midjourneyDbContext.MidjourneyVersion7.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 4" => await _midjourneyDbContext.MidjourneyVersionNiji4.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 5" => await _midjourneyDbContext.MidjourneyVersionNiji5.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                "niji 6" => await _midjourneyDbContext.MidjourneyVersionNiji6.FirstOrDefaultAsync(p => p.PropertyName == propertyName),
                _ => null
            };

            if (parameter == null)
                return Result.Fail<ParameterDetails>($"Parameter '{propertyName}' not found for version '{version}' or unsupported version");

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

            var exists = version switch
            {
                "1" => await _midjourneyDbContext.MidjourneyVersion1.AnyAsync(p => p.PropertyName == propertyName),
                "2" => await _midjourneyDbContext.MidjourneyVersion2.AnyAsync(p => p.PropertyName == propertyName),
                "3" => await _midjourneyDbContext.MidjourneyVersion3.AnyAsync(p => p.PropertyName == propertyName),
                "4" => await _midjourneyDbContext.MidjourneyVersion4.AnyAsync(p => p.PropertyName == propertyName),
                "5" => await _midjourneyDbContext.MidjourneyVersion5.AnyAsync(p => p.PropertyName == propertyName),
                "5.1" => await _midjourneyDbContext.MidjourneyVersion51.AnyAsync(p => p.PropertyName == propertyName),
                "5.2" => await _midjourneyDbContext.MidjourneyVersion52.AnyAsync(p => p.PropertyName == propertyName),
                "6" => await _midjourneyDbContext.MidjourneyVersion6.AnyAsync(p => p.PropertyName == propertyName),
                "6.1" => await _midjourneyDbContext.MidjourneyVersion61.AnyAsync(p => p.PropertyName == propertyName),
                "7" => await _midjourneyDbContext.MidjourneyVersion7.AnyAsync(p => p.PropertyName == propertyName),
                "niji 4" => await _midjourneyDbContext.MidjourneyVersionNiji4.AnyAsync(p => p.PropertyName == propertyName),
                "niji 5" => await _midjourneyDbContext.MidjourneyVersionNiji5.AnyAsync(p => p.PropertyName == propertyName),
                "niji 6" => await _midjourneyDbContext.MidjourneyVersionNiji6.AnyAsync(p => p.PropertyName == propertyName),
                _ => false
            };

            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Database error while checking parameter existence: {ex.Message}");
        }
    }
}
