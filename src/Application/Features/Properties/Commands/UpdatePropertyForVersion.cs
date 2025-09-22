using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Properties.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.Properties.Commands;

public static class UpdatePropertyForVersion
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

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var propertyName = PropertyName.Create(command.PropertyName);
            var parameters = command.Parameters.Select(Param.Create).ToList();
            var defaultValue = command.DefaultValue is not null ? DefaultValue.Create(command.DefaultValue) : null;
            var minValue = command.MinValue is not null ? MinValue.Create(command.MinValue) : null;
            var maxValue = command.MaxValue is not null ? MaxValue.Create(command.MaxValue) : null;
            var description = command.Description is not null ? Description.Create(command.Description) : null;

            var property = MidjourneyPropertiesBase.Create
            (
                propertyName.Value,
                version.Value,
                parameters,
                defaultValue?.Value,
                minValue?.Value,
                maxValue?.Value,
                description?.Value
            );

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(property)
                .ExecuteAndMapResultIfNoErrors(
                    () => _propertiesRepository.UpdateParameterForVersionAsync(property.Value, cancellationToken),
                    PropertyResponse.FromDomain
                );

            return result;
        }
    }
}