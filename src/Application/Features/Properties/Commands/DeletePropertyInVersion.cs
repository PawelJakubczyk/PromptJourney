using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Application.Features.Common.Responses;
using Application.Extension;

namespace Application.Features.Properties.Commands;

public static class DeletePropertyInVersion
{
    public sealed record Command(string Version, string PropertyName) : ICommand<DeleteResponse>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRepository propertiesRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var propertyName = PropertyName.Create(command.PropertyName);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(version)
                .CollectErrors(propertyName)
                .ExecuteAndMapResultIfNoErrors
                (
                    () => _propertiesRepository.DeleteParameterInVersionAsync(version.Value, propertyName.Value, cancellationToken),
                    _ => DeleteResponse.Success($"Property '{propertyName.Value.Value}' was successfully deleted from version '{version.Value.Value}'.")
                );

            return result;
        }
    }
}