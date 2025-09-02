using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Commands.AddTagToStyle;

public static class AddTagToStyle
{
    public sealed record Command(StyleName StyleName, Tag Tag) : ICommand<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(command.StyleName)
                .CollectErrors<Tag>(command.Tag);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository)
                .IfTagAlreadyExists(command.StyleName, command.Tag, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<MidjourneyStyle>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.AddTagToStyleAsync(command.StyleName, command.Tag);
        }
    }
}