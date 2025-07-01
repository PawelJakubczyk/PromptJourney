using Application.Abstractions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.GetMasterVersionByVersion;

public sealed class GetMasterVersionByVersionQueryHandler : IRequestHandler<GetMasterVersionByVersionQuery, Result<MidjourneyVersionsMaster>>
{
    private readonly IVersionRepository _versionRepository;

    public GetMasterVersionByVersionQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<MidjourneyVersionsMaster>> Handle(GetMasterVersionByVersionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.GetMasterVersionByVersionAsync(request.Version);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersionsMaster>($"Error retrieving version: {ex.Message}");
        }
    }
}