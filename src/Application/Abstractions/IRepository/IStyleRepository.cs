using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IStyleRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync();
    Task<Result<MidjourneyStyle>> GetStyleByNameAsync(string name);
    Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(string type);
    Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<string> tags);
    Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(string keyword);
    Task<Result<bool>> CheckStyleExistsAsync(string name);
    Task<Result<bool>> CheckTagExistsAsync(string tag);
    // For Commands
    Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style);
    Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style);
    Task<Result<MidjourneyStyle>> DeleteStyleAsync(string styleName);
    Task<Result<MidjourneyStyle>> AddTagToStyleAsync(string styleName, string tag);
    Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(string styleName, string tag);
    Task<Result<bool>> CheckTagExistsAsync(string styleName, string tag);
    Task<Result<MidjourneyStyle>> UpadteStyleDescription(string styleName, string description);
}
