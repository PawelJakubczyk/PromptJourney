using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

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

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);
                return Result.Fail<bool>(error);
            }

            return await _styleRepository.CheckTagExistsInStyleAsync(query.StyleName, query.Tag);
        }
    }
}
