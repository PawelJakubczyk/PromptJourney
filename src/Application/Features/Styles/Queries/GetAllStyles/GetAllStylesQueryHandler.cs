using Application.Abstractions;
using Application.Features.VersionsMaster.Queries.GetVersionByVersion;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetAllStyles;

public sealed class GetAllStylesQueryHandler : IRequestHandler<GetAllStylesQuery, Result<List<MidjourneyStyle>>>
{
    private readonly IStyleRepository _styleRepository;

    public GetAllStylesQueryHandler(IStyleRepository versionRepository)
    {
        _styleRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyStyle>>> Handle(GetAllStylesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _styleRepository.GetAllStylesAsync();
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Error retrieving all Styles. {ex.Message}");
        }
    }
}