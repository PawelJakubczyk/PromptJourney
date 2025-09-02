using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IStyleRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyle>>> GetAllStylesAsync();
    Task<Result<MidjourneyStyle>> GetStyleByNameAsync(StyleName name);
    Task<Result<List<MidjourneyStyle>>> GetStylesByTypeAsync(StyleType type);
    Task<Result<List<MidjourneyStyle>>> GetStylesByTagsAsync(List<Tag> tags);
    Task<Result<List<MidjourneyStyle>>> GetStylesByDescriptionKeywordAsync(Keyword keyword);
    Task<Result<bool>> CheckStyleExistsAsync(StyleName name);
    Task<Result<bool>> CheckTagExistsInStyleAsync(StyleName styleName, Tag tag);
    // For Commands
    Task<Result<MidjourneyStyle>> AddStyleAsync(MidjourneyStyle style);
    Task<Result<MidjourneyStyle>> UpdateStyleAsync(MidjourneyStyle style);
    Task<Result<MidjourneyStyle>> DeleteStyleAsync(StyleName styleName);
    Task<Result<MidjourneyStyle>> AddTagToStyleAsync(StyleName styleName, Tag tag);
    Task<Result<MidjourneyStyle>> DeleteTagFromStyleAsync(StyleName styleName, Tag tag);
    Task<Result<MidjourneyStyle>> UpadteStyleDescription(StyleName styleName, Description description);
}
