using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IVersionRepository
{
    // For Queries
    Task<Result<bool>> CheckVersionExistsInVersionsAsync(string version);
    Task<Result<MidjourneyVersions>> GetMasterVersionByVersionAsync(string version);
    Task<Result<List<MidjourneyVersions>>> GetAllVersions();
    // For Commands
    Task<Result<MidjourneyVersions>> AddVersionAsync(MidjourneyVersions version);
}
