using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;

namespace Application.Features.ExampleLinks.Commands;

public static class AddExampleLink
{
    public sealed record Command(ExampleLink Link, StyleName Style, ModelVersion Version) : ICommand<MidjourneyStyleExampleLink>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, MidjourneyStyleExampleLink>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyStyleExampleLink>> Handle(Command command, CancellationToken cancellationToken)
        {
            var linkResult = MidjourneyStyleExampleLink.Create
            (
                command.Link,
                command.Style,
                command.Version
            );

            var domainErrors = linkResult.Errors;

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(command.Version, _versionRepository)
                .IfStyleNotExists(command.Style, _styleRepository)
                .IfLinkAlreadyExists(command.Link, _exampleLinkRepository);

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<MidjourneyStyleExampleLink>(error);
            }

            return await _exampleLinkRepository.AddExampleLinkAsync(linkResult);
        }
    }
}