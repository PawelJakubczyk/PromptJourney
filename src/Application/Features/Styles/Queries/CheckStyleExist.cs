using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class CheckStyleExist
{
    public sealed record Query(string StyleName) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            List<DomainError> domainErrors = [];
            domainErrors
                .CollectErrors<StyleName>(styleName);

            var validationErrors = CreateValidationErrorIfAny<bool>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.CheckStyleExistsAsync(styleName.Value);
        }
    }
}