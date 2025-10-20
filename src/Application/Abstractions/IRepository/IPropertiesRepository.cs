using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPropertiesRepository
{
    // For Commands
    Task<Result<MidjourneyProperties>> AddPropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken);

    Task<Result<MidjourneyProperties>> DeletePropertyAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);

    Task<Result<MidjourneyProperties>> PatchPropertyAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken);

    Task<Result<MidjourneyProperties>> UpdatePropertyAsync(MidjourneyProperties property, CancellationToken cancellationToken);
 
    // For Queries
    Task<Result<bool>> CheckPropertyExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyProperties>>> GetAllPropertiesAsync(CancellationToken cancellationToken);

    Task<Result<List<MidjourneyProperties>>> GetAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken);
}
