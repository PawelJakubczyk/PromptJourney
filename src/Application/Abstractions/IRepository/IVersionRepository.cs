using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IVersionRepository
{
    // For Queries
    Task<Result<bool>> CheckVersionExistsInVersionsAsync(ModelVersion version, CancellationToken cancellationToken);
    Task<Result<bool>> CheckIfAnySupportedVersionExistsAsync(CancellationToken cancellationToken);
    Task<Result<MidjourneyVersion>> GetMasterVersionByVersionAsync(ModelVersion version, CancellationToken cancellationToken);
    Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken);
    Task<Result<List<ModelVersion>>> GetAllSuportedVersionsAsync(CancellationToken cancellationToken);

    // For Commands
    Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion version, CancellationToken cancellationToken);
}
