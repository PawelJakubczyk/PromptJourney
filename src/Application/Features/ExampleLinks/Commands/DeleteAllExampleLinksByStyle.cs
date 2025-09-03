using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Common.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteAllExampleLinksByStyle
{
    public sealed record Command(string StyleName) : ICommand<BulkDeleteResponse>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : ICommandHandler<Command, BulkDeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<BulkDeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(styleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(styleName.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<BulkDeleteResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var deleteResult = await _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleName);

            if (deleteResult.IsFailed) 
                return Result.Fail<BulkDeleteResponse>(deleteResult.Errors);

            var deletedCount = deleteResult.Value.Count;
            var response = BulkDeleteResponse.Success(deletedCount, 
                $"Successfully deleted {deletedCount} example links for style '{styleName.Value.Value}'.");

            return Result.Ok(response);
        }
    }
}