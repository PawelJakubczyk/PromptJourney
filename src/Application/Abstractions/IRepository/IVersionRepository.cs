using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IVersionRepository
{
    // For Queries
    Task<Result<bool>> CheckVersionExists(ModelVersion version, CancellationToken cancellationToken);
    Task<Result<bool>> CheckIfAnyVersionExistsAsync(CancellationToken cancellationToken);
    Task<Result<MidjourneyVersion>> GetVersion(ModelVersion version, CancellationToken cancellationToken);
    Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync(CancellationToken cancellationToken);
    Task<Result<List<string>>> GetAllSuportedVersionsAsync(CancellationToken cancellationToken);

    // For Commands
    Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion version, CancellationToken cancellationToken);
}