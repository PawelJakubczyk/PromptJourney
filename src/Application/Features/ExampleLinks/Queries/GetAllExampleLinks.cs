using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.ExampleLinks.Responses;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class GetAllExampleLinks
{
    public sealed record Query : IQuery<List<ExampleLinkRespose>>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : IQueryHandler<Query, List<ExampleLinkRespose>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<List<ExampleLinkRespose>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _exampleLinkRepository.GetAllExampleLinksAsync();

            if (result.IsFailed)
                return Result.Fail<List<ExampleLinkRespose>>(result.Errors);

            var responses = result.Value
                .Select(ExampleLinkRespose.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}