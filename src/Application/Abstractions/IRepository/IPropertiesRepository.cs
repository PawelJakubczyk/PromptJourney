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
    Task<Result<MidjourneyPropertiesBase>> AddParameterToVersionAsync(MidjourneyPropertiesBase property);
    Task<Result<MidjourneyPropertiesBase>> UpdateParameterForVersionAsync(MidjourneyPropertiesBase property);
    Task<Result<MidjourneyPropertiesBase>> PatchParameterForVersionAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue);
    Task<Result<MidjourneyPropertiesBase>> DeleteParameterInVersionAsync(ModelVersion version, PropertyName propertyName);
}