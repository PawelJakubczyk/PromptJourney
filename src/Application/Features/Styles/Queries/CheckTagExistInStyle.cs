using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public class CheckTagExistInStyle
{
    public sealed record Query(string StyleName, string Tag) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);
            var tag = Tag.Create(query.Tag);

            List<DomainError> domainErrors = [];
            domainErrors
                .CollectErrors<StyleName>(styleName)
                .CollectErrors<Tag>(tag);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfTagNotExists(styleName.Value, tag.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<bool>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.CheckTagExistsInStyleAsync(styleName.Value, tag.Value);
        }
    }
}
