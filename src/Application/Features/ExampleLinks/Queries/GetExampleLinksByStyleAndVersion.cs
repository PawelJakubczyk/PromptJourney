using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyleAndVersion
{
    public sealed record Query(StyleName Style, ModelVersion Version) : IQuery<List<MidjourneyStyleExampleLink>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository,
        IVersionRepository versionRepository
    ) : IQueryHandler<Query, List<MidjourneyStyleExampleLink>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<MidjourneyStyleExampleLink>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(query.Style)
                .CollectErrors<ModelVersion>(query.Version);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(query.Version, _versionRepository)
                .IfStyleNotExists(query.Style, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<List<MidjourneyStyleExampleLink>>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(query.Style, query.Version);
        }
    }
}