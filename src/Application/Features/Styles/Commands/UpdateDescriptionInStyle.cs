using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Commands;

public static class UpdateDescriptionInStyle
{
    public sealed record Command(string StyleName, string NewDescription) : ICommand<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(command.StyleName);
            var description = Description.Create(command.NewDescription);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(styleName)
                .CollectErrors<Description>(description);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(styleName.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<StyleResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.UpadteStyleDescription(styleName.Value, description.Value);

            if (result.IsFailed)
                return Result.Fail<StyleResponse>(result.Errors);

            var response = StyleResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}