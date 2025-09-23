using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

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

            var linkResult = MidjourneyStyleExampleLink.Create
            (
                link.Value,
                style.Value,
                version.Value
            );

            var result = await ValidationPipeline
                .EmptyAsync()
                    .CollectErrors(linkResult)
                    .BeginValidationBlock()
                        .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                        .IfStyleNotExists(style.Value, _styleRepository, cancellationToken)
                        .IfLinkAlreadyExists(link.Value, _exampleLinkRepository, cancellationToken)
                    .EndValidationBlock()
                    .IfNoErrors()
                        .Executes(() => _exampleLinkRepository.AddExampleLinkAsync(linkResult.Value, cancellationToken))
                            .MapResult(_ => new ExampleLinkResponse(command.Link, command.Style, command.Version));

            return result;
        }
    }
}