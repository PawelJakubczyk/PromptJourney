using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;

namespace Application.Abstractions.IRepository;

public interface IVersionRepository
{
    // For Commands
    Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion version, CancellationToken cancellationToken);

    Task<Result<MidjourneyVersion>> DeleteVersionAsync(ModelVersion version, CancellationToken cancellationToken);

    // For Queries
    Task<Result<bool>> CheckIfAnyVersionExistsAsync(CancellationToken cancellationToken);

    Task<Result<bool>> CheckVersionExistsAsync(ModelVersion version, CancellationToken cancellationToken);

    Task<Result<bool>> CheckParameterExistsAsync(Param parameter, CancellationToken cancellationToken);

    Task<Result<List<string>>> GetAllSupportedVersionsAsync(CancellationToken cancellationToken);

    Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken);

    Task<Result<MidjourneyVersion>> GetLatestVersionAsync(CancellationToken cancellationToken);

    Task<Result<MidjourneyVersion>> GetVersionAsync(ModelVersion version, CancellationToken cancellationToken);
}
