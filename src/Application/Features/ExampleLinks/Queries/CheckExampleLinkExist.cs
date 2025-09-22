using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class CheckExampleLinkExist
{
    public sealed record Query(string Link) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(query.Link);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(link)
                .ExecuteAndMapResultIfNoErrors(
                    () => _exampleLinksRepository.CheckExampleLinkExistsAsync(link.Value, cancellationToken),
                    _ => true
                );

            return result;
        }
    }
}
