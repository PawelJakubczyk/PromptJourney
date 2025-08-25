using Application.Features.Properties;
using Domain.Entities.MidjourneyProperties;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPropertiesRopository
{
    // For Queries
    Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionAsync(string version);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(string version, string propertyName);
    // For Commands
    Task<Result<PropertyDetails>> AddParameterToVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<PropertyDetails>> UpdateParameterForVersionAsync(string version, string propertyName, string[] parameters, string? defaultValue, string? minValue, string? maxValue, string? description);
    Task<Result<PropertyDetails>> PatchParameterForVersionAsync(string version, string propertyName, string characteristicToUpdate, string? newValue);
    Task<Result<PropertyDetails>> DeleteParameterInVersionAsync(string version, string propertyName);
}