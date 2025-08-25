using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ExampleLinks.Queries;

public class CheckExampleLinksAreNotEmpty
{
    public sealed record Query(string Link) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository) : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;
        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Link.Input.MustNotBeNullOrEmpty(query.Link);
            await Validate.Link.Input.MustHaveMaximumLength(query.Link);

            return await _exampleLinksRepository.CheckExampleLinksAreNotEmpty();
        }
    }
}
