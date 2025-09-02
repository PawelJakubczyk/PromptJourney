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

namespace Application.Features.Styles.Commands.RemoveStyle;

public static class DeleteStyle
{
    public sealed record Command(StyleName StyleName) : ICommand<MidjourneyStyle>;

    public sealed class Handler(
        IStyleRepository styleRepository,
        IExampleLinksRepository exampleLinksRepository
    ) : ICommandHandler<Command, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(command.StyleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<MidjourneyStyle>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            await _exampleLinksRepository.DeleteAllExampleLinksByStyleAsync(command.StyleName);

            return await _styleRepository.DeleteStyleAsync(command.StyleName);
        }
    }
}
