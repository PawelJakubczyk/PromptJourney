using Application.Abstractions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.GetAllVersions;

public sealed class GetStyleByTypeQueryHandler : IRequestHandler<GetStyleByTypeQuery, Result<List<MidjourneyVersionsMaster>>>
{
    private readonly IVersionRepository _versionRepository;

    public GetStyleByTypeQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyVersionsMaster>>> Handle(GetStyleByTypeQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.GetAllVersions();
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyVersionsMaster>>($"Error retrieving versions: {ex.Message}");
        }
    }
}