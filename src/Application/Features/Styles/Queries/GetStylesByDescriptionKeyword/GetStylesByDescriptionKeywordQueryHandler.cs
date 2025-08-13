using Application.Abstractions;
using Application.Features.VersionsMaster.Queries.GetVersionByVersion;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStylesByDescriptionKeyword;

public sealed class GetStylesByDescriptionKeywordQueryHandler : IRequestHandler<GetStylesByDescriptionKeywordQuery, Result<List<MidjourneyStyle>>>
{
    private readonly IStyleRepository _styleRepository;

    public GetStylesByDescriptionKeywordQueryHandler(IStyleRepository versionRepository)
    {
        _styleRepository = versionRepository;
    }

    public async Task<Result<List<MidjourneyStyle>>> Handle(GetStylesByDescriptionKeywordQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _styleRepository.GetStylesByDescriptionKeywordAsync(request.DescriptionKeyword);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyStyle>>($"Error retrieving Description Keyword: '{request.DescriptionKeyword}'. {ex.Message}");
        }
    }
}