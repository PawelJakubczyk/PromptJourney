using Application.Features.Properties;
using FluentResults;

namespace Application.Abstractions;

public interface IPropertiesRopository
{
    Task<Result<PropertyDetails>> AddParameterToVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<PropertyDetails>> UpdateParameterFromVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<PropertyDetails>> PatchParameterInVersionAsync(string version, string propertyName, string characteristicToUpdate, string? newValue);
    Task<Result<PropertyDetails>> DeleteParameterInVersionAsync(string version, string propertyName);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(string version, string propertyName);
}