using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions;

public interface IStyleRepository
{
    Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync();
    Task<Result<MidjourneyStyle>> GetStyleByNameAsync(string name);
    Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(string type);
    Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<string> tags);
    Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(string keyword);


    Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style);

    Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style);

    //Task<Result<List<MidjourneyPropertiesBase>>> GetAllParametersByVersionMasterAsync(string version);
    //Task<Result<bool>> CheckVersionExistsInVersionMasterAsync(string version);
    //Task<Result<MidjourneyVersionsMaster>> GetMasterVersionByVersionAsync(string version);
    //Task<Result<List<MidjourneyVersionsMaster>>> GetAllVersions();
}
