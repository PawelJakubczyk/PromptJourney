using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.ExampleLinks.Responses;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.ExampleLinks.Queries;

public static class GetAllExampleLinks
{
    public sealed record Query : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var getAllLinksResult = await _exampleLinksRepository.GetAllExampleLinksAsync();
            var persitanceErrors = getAllLinksResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<List<ExampleLinkResponse>>
            (
                (nameof(persitanceErrors), persitanceErrors)
            );

            if (validationErrors is not null) return validationErrors;

            var responses = getAllLinksResult.Value
                .Select(ExampleLinkResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}