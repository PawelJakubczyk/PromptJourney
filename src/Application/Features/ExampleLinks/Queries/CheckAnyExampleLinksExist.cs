using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class CheckAnyExampleLinksExist
{
    public sealed record Query() : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .ExecuteAndMapResultIfNoErrors(
                    () => _exampleLinksRepository.CheckAnyExampleLinksExistAsync(cancellationToken),
                    _ => true
                );

            return result;
        }
    }
}