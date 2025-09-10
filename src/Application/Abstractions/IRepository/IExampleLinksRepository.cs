using Domain.Entities.MidjourneyStyleExampleLinks;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;
public interface IExampleLinksRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync();
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleName);
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(StyleName styleName, ModelVersion version);
    Task<Result<bool>> CheckExampleLinkExistsAsync(ExampleLink link);
    Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(StyleName style);
    Task<Result<bool>> CheckExampleLinksAreNotEmpty();
    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink link);
    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(ExampleLink link);
    Task<Result<List<MidjourneyStyleExampleLink>>> DeleteAllExampleLinksByStyleAsync(StyleName style);
}
