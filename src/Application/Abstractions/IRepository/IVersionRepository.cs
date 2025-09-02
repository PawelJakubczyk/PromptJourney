using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IVersionRepository
{
    // For Queries
    Task<Result<bool>> CheckVersionExistsInVersionsAsync(ModelVersion version);
    Task<Result<bool>> CheckIfAnySupportedVersionExistsAsync();
    Task<Result<MidjourneyVersion>> GetMasterVersionByVersionAsync(ModelVersion version);
    Task<Result<List<MidjourneyVersion>>> GetAllVersionsAsync();
    Task<Result<List<string>>> GetAllSuportedVersionsAsync();

    // For Commands
    Task<Result<MidjourneyVersion>> AddVersionAsync(MidjourneyVersion version);
}
