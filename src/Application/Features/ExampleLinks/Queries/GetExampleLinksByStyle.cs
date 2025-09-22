using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.ExampleLinks.Responses;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyle
{
    public sealed record Query(string StyleName) : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .ExecuteAndMapResultIfNoErrors(
                    () => _exampleLinksRepository.GetExampleLinksByStyleAsync(styleName.Value, cancellationToken),
                    domainList => domainList.Select(ExampleLinkResponse.FromDomain).ToList()
                );

            return result;
        }

    }
}