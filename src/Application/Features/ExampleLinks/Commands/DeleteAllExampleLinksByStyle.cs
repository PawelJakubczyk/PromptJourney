using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities.MidjourneyStyles;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteAllExampleLinksByStyle
{
    public sealed record Command(string StyleName) : ICommand<List<ExampleLinkRespose>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : ICommandHandler<Command, List<ExampleLinkRespose>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<ExampleLinkRespose>>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleName>(command.StyleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(command.StyleName, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<List<ExampleLinkRespose>>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(command.StyleName);
        }
    }
}