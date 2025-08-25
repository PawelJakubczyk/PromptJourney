using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.ExampleLinks.Commands;

public static class AddExampleLink
{
    public sealed record Command(string Link, string Style, string Version) : ICommand<MidjourneyStyleExampleLink>;

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
            await Validate.Version.Input.MustNotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLength(command.Version);
            await Validate.Version.ShouldExists(command.Version, _versionRepository);

            await Validate.Style.Input.MustNotBeNullOrEmpty(command.Style);
            await Validate.Style.Input.MustHaveMaximumLenght(command.Style);
            await Validate.Style.ShouldExists(command.Style, _styleRepository);

            await Validate.Link.Input.MustNotBeNullOrEmpty(command.Link);
            await Validate.Link.Input.MustHaveMaximumLength(command.Link);
            await Validate.Link.ShouldNotExists(command.Link, _exampleLinkRepository);

            var link = MidjourneyStyleExampleLink.Create
            (
                command.Link,
                command.Style,
                command.Version
            );
            return await _exampleLinkRepository.AddExampleLinkAsync(link.Value);
        }
    }
}