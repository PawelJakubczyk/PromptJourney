using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Commands;

public static class DeleteVersion
{
    public sealed record Command(string Version) : ICommand<DeleteResponse>;

    public sealed class Handler
    (
        IVersionRepository versionRepository,
        HybridCache cache
    ) : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly HybridCache _cache = cache;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(version)
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _versionRepository.DeleteVersionAsync(version.Value, cancellationToken))
                .MapResult(() => DeleteResponse.Success
                    ($"Version '{version.Value.Value}' was successfully deleted."));

            return result;
        }
    }
}