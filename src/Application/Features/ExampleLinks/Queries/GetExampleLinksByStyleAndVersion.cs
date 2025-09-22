using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.ExampleLinks.Responses;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;



namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyleAndVersion
{
    public sealed record Query(string StyleName, string Version) : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository
    ) : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);
            var version = ModelVersion.Create(query.Version);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors<StyleName>(styleName)
                .CollectErrors<ModelVersion>(version)
                .ExecuteAndMapResultIfNoErrors(
                    () => _exampleLinksRepository.GetExampleLinksByStyleAndVersionAsync(styleName.Value, version.Value, cancellationToken),
                    domainList => domainList.Select(ExampleLinkResponse.FromDomain).ToList()
                );

            return result;
        }
    }
}