using Application.Abstractions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.GetMasterVersionByVersion;

public sealed class GetAllParametersByVersionMasterQueryHandler : IRequestHandler<GetAllParametersByVersionMasterQuery, Result<List<MidjourneyVersionsBase>>>
{
    private readonly IVersionRepository _versionRepository;

    public GetAllParametersByVersionMasterQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyVersionsBase>>> Handle(GetAllParametersByVersionMasterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.GetAllParametersByVersionMasterAsync(request.Version);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyVersionsBase>>($"Error retrieving version: {ex.Message}");
        }
    }
}