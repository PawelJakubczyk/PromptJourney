using Application.Abstractions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.GetVersionByVersion;

public sealed class GetVersionByVersionQueryHandler : IRequestHandler<GetVersionByVersionQuery, Result<MidjourneyVersionsMaster>>
{
    private readonly IVersionRepository _versionRepository;

    public GetVersionByVersionQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<MidjourneyVersionsMaster>> Handle(GetVersionByVersionQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.GetMasterVersionByVersionAsync(query.Version);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersionsMaster>($"Error retrieving version: {ex.Message}");
        }
    }
}