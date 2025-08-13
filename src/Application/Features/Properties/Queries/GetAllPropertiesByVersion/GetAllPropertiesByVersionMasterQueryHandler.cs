using Application.Abstractions;
using Domain.Entities.MidjourneyProperties;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Queries.GetAllPropertiesByVersion;

public sealed class GetAllPropertiesByVersionMasterQueryHandler : IRequestHandler<GetAllPropertiesByVersionMasterQuery, Result<List<MidjourneyPropertiesBase>>>
{
    private readonly IVersionRepository _versionRepository;

    public GetAllPropertiesByVersionMasterQueryHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyPropertiesBase>>> Handle(GetAllPropertiesByVersionMasterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.GetAllParametersByVersionMasterAsync(request.Version);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPropertiesBase>>($"Error retrieving version: {ex.Message}");
        }
    }
}