using Application.Features.Versions;
using Application.Features.Versions.Commands.AddParameterToVersion;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions;

public interface IVersionRepository
{
    // Version methods
    Task<Result<MidjourneyVersionsMaster>> GetMasterVersionByVersionAsync(string version);
    Task<Result<bool>> CheckVersionExistsInVersionMasterAsync(string version);

    // Parameter methods
    Task<Result<ParameterDetails>> AddParameterToVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<ParameterDetails>> UpdateParameterFromVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<ParameterDetails>> PatchParameterInVersionAsync(string version, string propertyName, string characteristicToUpdate, string? newValue);
    Task<Result<ParameterDetails>> DeleteParameterInVersionAsync(string version, string propertyName);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(string version, string propertyName);
}
