using Application.UseCases.Common.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IExampleLinksRepository
{
    // For Commands
    Task<Result<MidjourneyStyleExampleLink>> AddExampleLinkAsync(MidjourneyStyleExampleLink link, CancellationToken cancellationToken);

    Task<Result<BulkDeleteResponse>> DeleteAllExampleLinksByStyleAsync(StyleName style, CancellationToken cancellationToken);

    Task<Result<MidjourneyStyleExampleLink>> DeleteExampleLinkAsync(Guid Id, CancellationToken cancellationToken);

    // For Queries

    Task<Result<bool>> CheckAnyExampleLinksExistAsync(CancellationToken cancellationToken);

    Task<Result<bool>> CheckExampleLinkExistsByIdAsync(Guid Id, CancellationToken cancellationToken);

    Task<Result<bool>> CheckExampleLinkExistsByLinkAsync(ExampleLink link, CancellationToken cancellationToken);

    Task<Result<bool>> CheckExampleLinkExistsByStyleAsync(StyleName style, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyStyleExampleLink>>> GetAllExampleLinksAsync(CancellationToken cancellationToken);

    Task<Result<MidjourneyStyleExampleLink>> GetExampleLinkByIdAsync(Guid Id, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinkByLinkAsync(ExampleLink link, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAsync(StyleName styleName, CancellationToken cancellationToken);

    Task<Result<List<MidjourneyStyleExampleLink>>> GetExampleLinksByStyleAndVersionAsync(StyleName styleName, ModelVersion version, CancellationToken cancellationToken);
}
