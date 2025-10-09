using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IExampleLinksRepository
{
    // For Queries
    Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync(CancellationToken cancellationToken);

    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(StyleName styleName, ModelVersion version, CancellationToken cancellationToken);

    Task<Result<bool>> CheckExampleLinkExistsAsync(ExampleLink link, CancellationToken cancellationToken);

    Task<Result<bool>> CheckExampleLinkWithStyleExistsAsync(StyleName style, CancellationToken cancellationToken);

    Task<Result<bool>> CheckAnyExampleLinksExistAsync(CancellationToken cancellationToken);

    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink link, CancellationToken cancellationToken);

    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(ExampleLink link, CancellationToken cancellationToken);

    Task<Result<int>> DeleteAllExampleLinksByStyleAsync(StyleName style, CancellationToken cancellationToken);
}