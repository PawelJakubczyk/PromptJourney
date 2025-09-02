using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public class CheckTagExistInStyle
{
    public sealed record Query(StyleName StyleName, Tag Tag) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];
            domainErrors
                .CollectErrors<StyleName>(query.StyleName)
                .CollectErrors<Tag>(query.Tag);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfTagNotExists(query.StyleName, query.Tag, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<bool>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.CheckTagExistsInStyleAsync(query.StyleName, query.Tag);
        }
    }
}
