using Application.Features.Properties;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPropertiesRepository
{
    // For Queries
    Task<Result<List<MidjourneyProperties>>> GetAllParametersByVersionAsync(ModelVersion version, CancellationToken cancellationToken);
    Task<Result<bool>> CheckParameterExistsInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);
    // For Commands
    Task<Result<MidjourneyProperties>> AddParameterToVersionAsync(MidjourneyProperties property, CancellationToken cancellationToken);
    Task<Result<MidjourneyProperties>> UpdateParameterForVersionAsync(MidjourneyProperties property, CancellationToken cancellationToken);
    Task<Result<MidjourneyProperties>> PatchParameterForVersionAsync(ModelVersion version, PropertyName propertyName, string characteristicToUpdate, string? newValue, CancellationToken cancellationToken);
    Task<Result<MidjourneyProperties>> DeleteParameterInVersionAsync(ModelVersion version, PropertyName propertyName, CancellationToken cancellationToken);
}