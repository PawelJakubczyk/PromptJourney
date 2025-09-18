using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.ExampleLinks.Responses;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyleAndVersion
{
    public sealed record Query(string Style, string Version) : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository
    ) : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var style = StyleName.Create(query.Style);
            var version = ModelVersion.Create(query.Version);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(style)
                .CollectErrors<ModelVersion>(version);

            var getExamleLinksResult = await _exampleLinksRepository.GetExampleLinksByStyleAndVersionAsync(style.Value, version.Value);
            var persitanceErrors = getExamleLinksResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<List<ExampleLinkResponse>>
            (
                (nameof(domainErrors), domainErrors),
                (nameof(persitanceErrors), persitanceErrors)
            );

            if (validationErrors is not null) return validationErrors;

            var responses = getExamleLinksResult.Value
                .Select(ExampleLinkResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}