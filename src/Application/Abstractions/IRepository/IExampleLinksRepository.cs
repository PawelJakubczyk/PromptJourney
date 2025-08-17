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
    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink link);
    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(string style);
    Task<Result<List<MidjourneyStyleExampleLink>>> DeleteAllExampleLinkWithStyleAsync(string style);
}
