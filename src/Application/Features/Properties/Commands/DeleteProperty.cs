using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.Common.Responses;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Workflows;

namespace Application.Features.Properties.Commands;

public static class DeleteProperty
{
    public sealed record Command(string Version, string PropertyName) : ICommand<DeleteResponse>;

    public sealed class Handler
    (
        IVersionRepository versionRepository,
        IPropertiesRepository propertiesRepository,
        HybridCache cache
    ) : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly HybridCache _cache = cache;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .Validate(pipeline => pipeline
                    .CollectErrors(versionResult)
                    .CollectErrors(propertyNameResult))
                .Validate(pipeline => pipeline
                    .IfVersionNotExists(versionResult.Value, _versionRepository, cancellationToken)
                    .IfPropertyNotExists(propertyNameResult.Value, versionResult.Value, _propertiesRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .DeletePropertyAsync(versionResult.Value, propertyNameResult.Value, cancellationToken))
                .MapResult(() => DeleteResponse.Success
                    ($"Property '{propertyNameResult.Value.Value}' was successfully deleted from version '{versionResult.Value.Value}'."));

            return result;
        }
    }
}