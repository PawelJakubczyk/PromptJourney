using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Domain.Errors.DomainError;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.ExampleLinks.Queries;

public class CheckExampleLinkExist
{
    public sealed record Query(string Link) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository) : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;
        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(query.Link);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ExampleLink>(link);

            var validationErrors = CreateValidationErrorIfAny<bool>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _exampleLinksRepository.CheckExampleLinkExistsAsync(link.Value);
        }
    }
}
