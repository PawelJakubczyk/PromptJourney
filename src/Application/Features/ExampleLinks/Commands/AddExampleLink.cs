using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.Features.ExampleLinks.Commands;

public static class AddExampleLink
{
    public sealed record Command(string Link, string StyleName, string Version) : ICommand<ExampleLinkResponse>;

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
            var styleName = StyleName.Create(command.StyleName);
            var version = ModelVersion.Create(command.Version);

            var linkResult = MidjourneyStyleExampleLink.Create
            (
                link.Value,
                styleName.Value,
                version.Value
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                    .CollectErrors(linkResult)
                    .Validate(pipeline => pipeline
                        .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                        .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                        .IfVersionNotInSuportedVersions(version.Value, _versionRepository, cancellationToken)
                        .IfLinkAlreadyExists(link.Value, _exampleLinkRepository, cancellationToken))
                    .ExecuteIfNoErrors(() => _exampleLinkRepository.AddExampleLinkAsync(linkResult.Value, cancellationToken))
                    .MapResult(_ => new ExampleLinkResponse(command.Link, command.StyleName, command.Version));

            return result;
        }
    }
}

