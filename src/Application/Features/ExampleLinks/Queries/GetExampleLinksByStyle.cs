using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.ExampleLinks.Responses;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyle
{
    public sealed record Query(string StyleName) : IQuery<List<ExampleLinkRespose>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : IQueryHandler<Query, List<ExampleLinkRespose>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<ExampleLinkRespose>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(styleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(styleName.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<List<ExampleLinkRespose>>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _exampleLinkRepository.GetExampleLinksByStyleAsync(styleName);

            if (result.IsFailed)
                return Result.Fail<List<ExampleLinkRespose>>(result.Errors);

            var responses = result.Value
                .Select(ExampleLinkRespose.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}