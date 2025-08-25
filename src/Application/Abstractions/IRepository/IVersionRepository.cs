using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IVersionRepository
{
    // For Queries
    Task<Result<bool>> CheckVersionExistsInVersionsAsync(string version);
    Task<Result<bool>> CheckIfAnySupportedVersionExistsAsync();
    Task<Result<MidjourneyVersions>> GetMasterVersionByVersionAsync(string version);
    Task<Result<List<MidjourneyVersions>>> GetAllVersionsAsync();
    Task<Result<List<string>>> GetAllSuportedVersionsAsync();

    // For Commands
    Task<Result<MidjourneyVersions>> AddVersionAsync(string version, string parameter, DateTime? releaseDate = null, string? description = null);
}
