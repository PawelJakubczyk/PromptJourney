using Application.Features.Properties;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPropertiesRepository
{
    // For Queries
    Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionAsync(ModelVersion version, CancellationToken cancellationToken);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);
    // For Commands
    Task<Result<MidjourneyPropertiesBase>> AddParameterToVersionAsync(MidjourneyPropertiesBase property, CancellationToken cancellationToken);
    Task<Result<MidjourneyPropertiesBase>> UpdateParameterForVersionAsync(MidjourneyPropertiesBase property, CancellationToken cancellationToken);
    Task<Result<MidjourneyPropertiesBase>> PatchParameterForVersionAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken);
    Task<Result<MidjourneyPropertiesBase>> DeleteParameterInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);
}