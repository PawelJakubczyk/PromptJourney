using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Properties.Queries;

public static class CheckPropertyExistsInVersion
{
    public sealed record Query(string Version, string PropertyName) : IQuery<bool>;

    public sealed class Handler(IPropertiesRepository propertiesRepository) : IQueryHandler<Query, bool>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);
            var propertyName = PropertyName.Create(query.PropertyName);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(version)
                .CollectErrors<PropertyName>(propertyName);

            var validationErrors = CreateValidationErrorIfAny<bool>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _propertiesRepository.CheckParameterExistsInVersionAsync(version.Value, propertyName.Value);
        }
    }
}