using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;

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

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(linkResult)
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken, true)
                .IfStyleNotExists(style.Value, _styleRepository, cancellationToken)
                .IfLinkAlreadyExists(link.Value, _exampleLinkRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors
                (
                    () => _exampleLinkRepository.AddExampleLinkAsync(linkResult.Value, cancellationToken),
                    _ => new ExampleLinkResponse(command.Link, command.Style, command.Version)
                );

            return result;

            var result2 = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(linkResult)
                .BeginValidationBlock()
                    .IfVersionNotExists(version.Value, _versionRepository, cancellationToken, true)
                    .IfStyleNotExists(style.Value, _styleRepository, cancellationToken)
                    .IfLinkAlreadyExists(link.Value, _exampleLinkRepository, cancellationToken)
                .EndValidationBlock()
                .ExecuteAndMapResultIfNoErrors
                (
                    () => _exampleLinkRepository.AddExampleLinkAsync(linkResult.Value, cancellationToken),
                    _ => new ExampleLinkResponse(command.Link, command.Style, command.Version)
                );

            return result;
        }
    }
}