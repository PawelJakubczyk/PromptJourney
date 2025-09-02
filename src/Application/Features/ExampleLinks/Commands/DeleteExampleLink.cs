using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using Domain.Errors;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.ExampleLinks.Commands;

public static class DeleteExampleLink
{
    public sealed record Command(ExampleLink Link) : ICommand<MidjourneyStyleExampleLink>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : ICommandHandler<Command, MidjourneyStyleExampleLink>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<MidjourneyStyleExampleLink>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ExampleLink>(command.Link);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfLinkNotExists(command.Link, _exampleLinkRepository);

            var validationErrors = CreateValidationErrorIfAny<MidjourneyStyleExampleLink>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _exampleLinkRepository.DeleteExampleLinkAsync(command.Link);
        }
    }
}