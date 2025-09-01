using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;
public interface IExampleLinksRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync();
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(Result<StyleName> styleNameResult);
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(Result<StyleName> styleNameResult, Result<ModelVersion> versionResult);
    Task<Result<bool>> CheckExampleLinkExistsAsync(Result<ExampleLink> linkResult);
    Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(Result<StyleName> styleResult);
    Task<Result<bool>> CheckExampleLinksAreNotEmpty();
    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(Result<MidjourneyStyleExampleLink> linkResult);
    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(Result<ExampleLink> linkResult);
    Task<Result<List<MidjourneyStyleExampleLink>>> DeleteAllExampleLinksByStyleAsync(Result<StyleName> styleResult);
}
