using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.ExampleLinks.Queries;

public class CheckExampleLinkWithStyleExists
{
    public sealed record Query(StyleName StyleName) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository) : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;
        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(query.StyleName);

            if (domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<bool>(error);
            }

            return await _exampleLinksRepository.CheckExampleLinkWithStyleExistsAsync(query.StyleName);
        }
    }
}
