using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Versions.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Commands;

public static class AddVersion
{
    public sealed record Command
    (
        string Version,
        string Parameter,
        string ReleaseDate,
        string? Description = null
    ) : ICommand<VersionResponse>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var parameter = Param.Create(command.Parameter);
            var releaseDate = ReleaseDate.Create(command.ReleaseDate);
            var description = command.Description is not null ? Description.Create(command.Description) : Result.Ok(Description.None);
            var midjourneyVersion = MidjourneyVersion.Create
            (
                version,
                parameter,
                releaseDate,
                description
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(midjourneyVersion)
                .IfVersionAlreadyExists(version.Value, _versionRepository, cancellationToken)
                .IfParamterAlreadyExists(parameter.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _versionRepository
                    .AddVersionAsync(midjourneyVersion.Value, cancellationToken))
                .MapResult(() => VersionResponse.FromDomain(midjourneyVersion.Value));

            return result;
        }
    }
}
