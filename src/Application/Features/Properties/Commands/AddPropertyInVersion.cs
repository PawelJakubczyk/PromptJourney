using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.Properties.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using System;
using Utilities.Validation;

namespace Application.Features.Properties.Commands;

public static class AddPropertyInVersion
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        List<string> Parameters,
        string? DefaultValue,
        string? MinValue,
        string? MaxValue,
        string? Description
    ) : ICommand<PropertyResponse>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRepository propertiesRepository)
        : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);
            var parametersResult = command.Parameters.Select(Param.Create).ToList();
            var defaultValueResult = command.DefaultValue is not null ? DefaultValue.Create(command.DefaultValue) : null;
            var minValueResult = command.MinValue is not null ? MinValue.Create(command.MinValue) : null;
            var maxValueResult = command.MaxValue is not null ? MaxValue.Create(command.MaxValue) : null;
            var descriptionResult = command.Description is not null ? Description.Create(command.Description) : null;

            var property = MidjourneyPropertiesBase.Create
            (
                propertyNameResult,
                versionResult,
                parametersResult,
                defaultValueResult!,
                minValueResult!,
                maxValueResult!,
                descriptionResult!
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(property)
                .Validate(pipeline => pipeline
                    .IfVersionNotExists(versionResult.Value, _versionRepository, cancellationToken)
                    .IfPropertyAlreadyExists(propertyNameResult.Value, versionResult.Value, _propertiesRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _propertiesRepository.AddParameterToVersionAsync(property.Value, cancellationToken))
                .MapResult(PropertyResponse.FromDomain);

            return result;
        }

    }
}