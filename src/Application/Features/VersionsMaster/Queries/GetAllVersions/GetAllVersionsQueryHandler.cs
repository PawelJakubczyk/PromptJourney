using Application.Abstractions;
using Application.Features.VersionsMaster.Queries.GetVersionByVersion;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.GetAllVersions;

public sealed class GetAllVersionsQueryHandler : IRequestHandler<GetAllVersionsQuery, Result<List<MidjourneyVersionsMaster>>>
{
    private readonly IVersionRepository _versionRepository;

    public GetAllVersionsQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyVersionsMaster>> Handle(GetAllVersionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.GetAllVersions();
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyVersionsMaster>>($"Error retrieving version: {ex.Message}");
        }
    }
}