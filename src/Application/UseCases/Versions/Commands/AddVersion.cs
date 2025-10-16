using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Commands;

public static class AddVersion
{
    public sealed record Command
    (
        string Version,
        string Parameter,
        DateTime? ReleaseDate = null,
        string? Description = null
    ) : ICommand<string>;

    public sealed class Handler(IVersionRepository versionRepository, HybridCache cache) : ICommandHandler<Command, string>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly HybridCache _cache = cache;

        public async Task<Result<string>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var parameter = Param.Create(command.Parameter);
            var description = command.Description is not null ? Description.Create(command.Description) : null;

            var midjourneyVersion = MidjourneyVersion.Create
            (
                version,
                parameter,
                command.ReleaseDate,
                description!
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(midjourneyVersion)
                .IfVersionAlreadyExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _versionRepository
                    .AddVersionAsync(midjourneyVersion.Value, cancellationToken))
                .MapResult
                    (() => command.Version);

            return result;
        }
    }
}
