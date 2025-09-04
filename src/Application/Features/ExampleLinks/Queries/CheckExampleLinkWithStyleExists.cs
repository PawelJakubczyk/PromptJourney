using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.ExampleLinks.Queries;

public class CheckExampleLinkWithStyleExists
{
    public sealed record Query(string StyleName) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository) : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;
        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(styleName);

            var validationErrors = CreateValidationErrorIfAny<bool>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _exampleLinksRepository.CheckExampleLinkWithStyleExistsAsync(styleName.Value);
        }
    }
}
