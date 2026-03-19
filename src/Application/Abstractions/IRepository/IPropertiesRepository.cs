using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;

namespace Application.Abstractions.IRepository;

public interface IPropertiesRepository
{
    // For Commands
    Task<Result<MidjourneyProperty>> AddPropertyAsync(MidjourneyProperty property, CancellationToken cancellationToken);

    Task<Result<MidjourneyProperty>> DeletePropertyAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);

    Task<Result<MidjourneyProperty>> PatchPropertyAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken);

    Task<Result<MidjourneyProperty>> UpdatePropertyAsync(MidjourneyProperty property, CancellationToken cancellationToken);
 
    // For Queries
    Task<Result<bool>> CheckPropertyExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyProperty>>> GetAllPropertiesAsync(CancellationToken cancellationToken);

    Task<Result<List<MidjourneyProperty>>> GetAllPropertiesByVersionAsync(ModelVersion version, CancellationToken cancellationToken);
}
