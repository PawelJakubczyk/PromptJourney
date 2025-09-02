using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.ExampleLinks.Commands;

public static class AddExampleLink
{
    public sealed record Command(string Link, string Style, string Version) : ICommand<ExampleLinkRespose>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, ExampleLinkRespose>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<ExampleLinkRespose>> Handle(Command command, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(command.Link);
            var style = StyleName.Create(command.Style);
            var version = ModelVersion.Create(command.Version);

            var linkResult = MidjourneyStyleExampleLink.Create(link.Value, style.Value, version.Value);
            var domainErrors = linkResult.Errors;

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository)
                .IfStyleNotExists(style.Value, _styleRepository)
                .IfLinkAlreadyExists(link.Value, _exampleLinkRepository);

            var validationErrors = CreateValidationErrorIfAny<ExampleLinkRespose>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var exampleLink = await _exampleLinkRepository.AddExampleLinkAsync(linkResult);

            if (exampleLink.IsFailed) return Result.Fail<ExampleLinkRespose>(exampleLink.Errors);

            var dto = ExampleLinkRespose.FromDomain(exampleLink.Value);

            return Result.Ok(dto);
        }
    }
}