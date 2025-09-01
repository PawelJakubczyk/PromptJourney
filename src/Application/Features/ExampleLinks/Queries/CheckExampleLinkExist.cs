using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.ExampleLinks.Queries;

public class CheckExampleLinkExist
{
    public sealed record Query(ExampleLink Link) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository) : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;
        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ExampleLink>(query.Link);

            if (domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<bool>(error);
            }

            return await _exampleLinksRepository.CheckExampleLinkExistsAsync(query.Link);
        }
    }
}
