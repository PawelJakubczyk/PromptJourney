using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public class CheckExampleLinkExist
{
    public sealed record Query(string Link) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository) : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;
        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Link.Input.MustNotBeNullOrEmpty(query.Link);
            await Validate.Link.Input.MustHaveMaximumLength(query.Link);

            return await _exampleLinksRepository.CheckExampleLinkExistsAsync(query.Link);
        }
    }
}
