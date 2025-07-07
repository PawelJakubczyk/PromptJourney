using Application.Features.Versions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions;

public interface IVersionRepository
{
    // VersionMaster methods
    Task<Result<List<MidjourneyVersionsBase>>> GetAllParametersByVersionMasterAsync(string version);
    Task<Result<bool>> CheckVersionExistsInVersionMasterAsync(string version);
    Task<Result<MidjourneyVersionsMaster>> GetMasterVersionByVersionAsync(string version);
    Task<Result<MidjourneyVersionsMaster>> (string version, string? description);

    // Versions methods
    Task<Result<ParameterDetails>> AddParameterToVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<ParameterDetails>> UpdateParameterFromVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<ParameterDetails>> PatchParameterInVersionAsync(string version, string propertyName, string characteristicToUpdate, string? newValue);
    Task<Result<ParameterDetails>> DeleteParameterInVersionAsync(string version, string propertyName);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(string version, string propertyName);
}
