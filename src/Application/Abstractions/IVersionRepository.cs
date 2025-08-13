using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions;

public interface IVersionRepository
{
    Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionMasterAsync(string version);
    Task<Result<bool>> CheckVersionExistsInVersionMasterAsync(string version);
    Task<Result<MidjourneyVersionsMaster>> GetMasterVersionByVersionAsync(string version);
    Task<Result<List<MidjourneyVersionsMaster>>> GetAllVersions();

    Task<Result<MidjourneyVersionsMaster>> AddVersionAsync(MidjourneyVersionsMaster version);
}
