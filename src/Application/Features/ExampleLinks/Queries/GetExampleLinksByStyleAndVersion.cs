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
    public sealed record Query(string Style, string Version) : IQuery<List<ExampleLinkRespose>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository,
        IVersionRepository versionRepository
    ) : IQueryHandler<Query, List<ExampleLinkRespose>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<ExampleLinkRespose>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var style = StyleName.Create(query.Style);
            var version = ModelVersion.Create(query.Version);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(style)
                .CollectErrors<ModelVersion>(version);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository)
                .IfStyleNotExists(style.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<List<ExampleLinkRespose>>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(style.Value, version.Value);

            if (result.IsFailed)
                return Result.Fail<List<ExampleLinkRespose>>(result.Errors);

            var responses = result.Value
                .Select(ExampleLinkRespose.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}