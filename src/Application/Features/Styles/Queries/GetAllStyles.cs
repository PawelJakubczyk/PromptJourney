using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using FluentResults;

namespace Application.Features.Styles.Queries;

public static class GetAllStyles
{
    public sealed record Query : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _styleRepository.GetAllStylesAsync();

            if (result.IsFailed)
                return Result.Fail<List<StyleResponse>>(result.Errors);

            var responses = result.Value
                .Select(StyleResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}