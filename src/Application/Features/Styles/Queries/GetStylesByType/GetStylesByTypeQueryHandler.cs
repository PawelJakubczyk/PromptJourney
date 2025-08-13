using Application.Abstractions;
using Domain.Entities.MidjourneyStyles;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStyleByType;

public sealed class GetStylesByTypeQueryHandler : IRequestHandler<GetStylesByTypeQuery, Result<List<MidjourneyStyle>>>
{
    private readonly IStyleRepository _styleRepository;

    public GetStylesByTypeQueryHandler(IStyleRepository versionRepository)
    {
        _styleRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyStyle>>> Handle(GetStylesByTypeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _styleRepository.GetStylesByTypeAsync(request.Type);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Error retrieving Style by Type: '{request.Type}'. {ex.Message}");
        }
    }
}