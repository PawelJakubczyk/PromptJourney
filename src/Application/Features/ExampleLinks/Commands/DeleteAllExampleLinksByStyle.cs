using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Common.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteAllExampleLinksByStyle
{
    public sealed record Command(string StyleName) : ICommand<BulkDeleteResponse>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository
    ) : ICommandHandler<Command, BulkDeleteResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<BulkDeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(styleName);

            var deleteLinksResult = await _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleName.Value);
            var persitanceErrors = deleteLinksResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<BulkDeleteResponse>
            (
                (nameof(domainErrors), domainErrors),
                (nameof(persitanceErrors), persitanceErrors)
            );

            if (validationErrors is not null) return validationErrors;

            var deletedCount = deleteLinksResult.Value.Count;
            var response = BulkDeleteResponse.Success
            (
                deletedCount, 
                $"Successfully deleted {deletedCount} example links for style '{styleName.Value.Value}'."
            );

            return Result.Ok(response);
        }
    }
}