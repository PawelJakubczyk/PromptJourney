using Application.Abstractions;
using Application.Features.VersionsMaster.Queries.GetVersionByVersion;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStyleByName;

public sealed class GetStylesByNameQueryHandler : IRequestHandler<GetStylesByNameQuery, Result<MidjourneyStyle>>
{
    private readonly IStyleRepository _styleRepository;

    public GetStylesByNameQueryHandler(IStyleRepository versionRepository)
    {
        _styleRepository = versionRepository;
    }

    public async Task<Result<MidjourneyStyle>> Handle(GetStylesByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _styleRepository.GetStyleByNameAsync(request.Name);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyStyle>($"Error retrieving Style by Name: '{request.Name}'. {ex.Message}");
        }
    }
}