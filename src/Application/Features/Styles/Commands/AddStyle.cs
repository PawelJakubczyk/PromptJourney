using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Commands.AddStyle;

public static class AddStyle
{
    public sealed record Command
    (
        StyleName Name,
        StyleType Type,
        Description? Description = null,
        List<Tag>? Tags = null
    ) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleAlreadyExists(command.Name, _styleRepository);

            var styleResult = MidjourneyStyle.Create(
                command.Name,
                command.Type,
                command.Description,
                command.Tags
            );

            var domainErrors = styleResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<MidjourneyStyle>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.AddStyleAsync(styleResult.Value);
        }
    }
}