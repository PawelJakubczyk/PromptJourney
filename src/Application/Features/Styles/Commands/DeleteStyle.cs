using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;
using Application.Features.Common.Responses;

namespace Application.Features.Styles.Commands.RemoveStyle;

public static class DeleteStyle
{
    public sealed record Command(string StyleName) : ICommand<DeleteResponse>;

    public sealed class Handler(
        IStyleRepository styleRepository,
        IExampleLinksRepository exampleLinksRepository
    ) : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(styleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(styleName.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<DeleteResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            await _exampleLinksRepository.DeleteAllExampleLinksByStyleAsync(styleName.Value);

            var result = await _styleRepository.DeleteStyleAsync(styleName.Value);

            if (result.IsFailed)
                return Result.Fail<DeleteResponse>(result.Errors);

            var response = DeleteResponse.Success($"Style '{styleName.Value.Value}' was successfully deleted.");

            return Result.Ok(response);
        }
    }
}
