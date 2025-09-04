using Domain.Entities.MidjourneyStyleExampleLinks;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;
public interface IExampleLinksRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync();
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleNameResult);
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(StyleName styleNameResult, ModelVersion versionResult);
    Task<Result<bool>> CheckExampleLinkExistsAsync(ExampleLink linkResult);
    Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(StyleName styleResult);
    Task<Result<bool>> CheckExampleLinksAreNotEmpty();
    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink linkResult);
    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(ExampleLink linkResult);
    Task<Result<List<MidjourneyStyleExampleLink>>> DeleteAllExampleLinksByStyleAsync(StyleName styleResult);
}
