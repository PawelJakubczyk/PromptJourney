using Application.Features.Properties;
using Domain.Entities.MidjourneyProperties;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPropertiesRepository
{
    // For Queries
    Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionAsync(ModelVersion version);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(ModelVersion version, PropertyName propertyName);
    // For Commands
    Task<Result<PropertyDetails>> AddParameterToVersionAsync(MidjourneyPropertiesBase property);
    Task<Result<PropertyDetails>> UpdateParameterForVersionAsync(MidjourneyPropertiesBase property);
    Task<Result<PropertyDetails>> PatchParameterForVersionAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue);
    Task<Result<PropertyDetails>> DeleteParameterInVersionAsync(ModelVersion version, PropertyName propertyName);
}