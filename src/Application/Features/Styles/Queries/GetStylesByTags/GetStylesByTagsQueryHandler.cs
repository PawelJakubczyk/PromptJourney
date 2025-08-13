using Application.Abstractions;
using Domain.Entities.MidjourneyStyles;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStylesByTag;

public sealed class GetStylesByTagsQueryHandler : IRequestHandler<GetStylesByTagsQuery, Result<List<MidjourneyStyle>>>
{
    private readonly IStyleRepository _styleRepository;

    public GetStylesByTagsQueryHandler(IStyleRepository versionRepository)
    {
        _styleRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyStyle>>> Handle(GetStylesByTagsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _styleRepository.GetStylesByTagsAsync(request.Tags);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Error retrieving Style By Tags: '{request.Tags}'. {ex.Message}");
        }
    }
}