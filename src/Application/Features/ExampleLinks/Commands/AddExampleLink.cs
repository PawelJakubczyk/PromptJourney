using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.ExampleLinks.Commands;

public static class AddExampleLink
{
    public sealed record Command(string Link, string Style, string Version) : ICommand<ExampleLinkResponse>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, ExampleLinkResponse>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<ExampleLinkResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(command.Link);
            var style = StyleName.Create(command.Style);
            var version = ModelVersion.Create(command.Version);

            var creatLinkResult = MidjourneyStyleExampleLink.Create
            (
                link.Value, 
                style.Value, 
                version.Value
            );

            var domainErrors = creatLinkResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<ExampleLinkResponse>
            (
                (nameof(domainErrors), domainErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var applicationErrors = new List<ApplicationError>();

            var styleExists = await _styleRepository.CheckStyleExistsAsync(style.Value, cancellationToken);
            var versionExists = await _versionRepository.CheckVersionExistsInVersionsAsync(version.Value, cancellationToken);
            var linkExists = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link.Value, cancellationToken);

            applicationErrors
                .IfStyleNotExists(styleExists)
                .IfVersionNotExist(versionExists)
                .IfLinkExists(linkExists);

            var persistenceErrors = new List<IError>();

            persistenceErrors
                .ColectErrors(styleExists)
                .ColectErrors(versionExists)
                .ColectErrors(linkExists);

            validationErrors = CreateValidationErrorIfAny<ExampleLinkResponse>
            (
                (nameof(applicationErrors), applicationErrors),
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var addExampleLinkResult = await _exampleLinkRepository.AddExampleLinkAsync(creatLinkResult.Value, cancellationToken);
            persistenceErrors = [.. addExampleLinkResult.Errors];

            validationErrors = CreateValidationErrorIfAny<ExampleLinkResponse>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var responses = ExampleLinkResponse.FromDomain(addExampleLinkResult.Value);

            return Result.Ok(responses);
        }
    }
}