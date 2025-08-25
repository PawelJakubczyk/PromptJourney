using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Abstractions.IRepository;
public interface IExampleLinksRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync();
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(string styleName);
    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(string styleName, string version);
    Task<Result<bool>> CheckExampleLinkExistsAsync(string link);
    Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(string link);
    Task<Result<bool>> CheckExampleLinksAreNotEmpty();
    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink link);
    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(string style);
    Task<Result<List<MidjourneyStyleExampleLink>>> DeleteAllExampleLinkByStyleAsync(string style);
}
