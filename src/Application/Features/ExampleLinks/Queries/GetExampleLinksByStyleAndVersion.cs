using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.ExampleLinks.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;



namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyleAndVersion
{
    public sealed record Query(string StyleName, string Version) : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository _styleRepository,
        IVersionRepository _versionRepository
    ) : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = _styleRepository;
        private readonly IVersionRepository _versionRepository = _versionRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);
            var version = ModelVersion.Create(query.Version);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .Validate(pipeline => pipeline
                    .CollectErrors(styleName)
                    .CollectErrors(version))
                .Validate(pipeline => pipeline
                    .IfStyleNotExists(styleName?.Value!, _styleRepository, cancellationToken)
                    .IfVersionNotExists(version?.Value!, _versionRepository, cancellationToken)
                    .IfVersionNotInSuportedVersions(version?.Value!, _versionRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _exampleLinksRepository.GetExampleLinksByStyleAndVersionAsync(styleName.Value, version.Value, cancellationToken))
                .MapResult
                (
                    domainList => domainList
                    .Select(ExampleLinkResponse.FromDomain)
                    .ToList()
                );


            return result;
        }
    }
}