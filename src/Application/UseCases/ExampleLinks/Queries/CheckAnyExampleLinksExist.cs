using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class CheckAnyExampleLinksExist
{
    public sealed record Query : IQuery<bool>
    {
        public static readonly Query Singletone = new();
    };

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _exampleLinksRepository
                    .CheckAnyExampleLinksExistAsync(cancellationToken);

            return result;
        }
    }
}
