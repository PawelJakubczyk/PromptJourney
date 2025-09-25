using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.VersionsMaster.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.VersionsMaster.Commands;

public static class AddVersion
{
    public sealed record Command
    (
        string Version,
        string Parameter,
        DateTime? ReleaseDate = null,
        string? Description = null
    ) : ICommand<VersionResponse>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Command command, CancellationToken cancellationToken)
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
                .ExecuteIfNoErrors(() => _versionRepository.AddVersionAsync(midjourneyVersion.Value, cancellationToken))
                .MapResult(VersionResponse.FromDomain);


            return result;
        }
    }

}