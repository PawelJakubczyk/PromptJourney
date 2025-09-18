using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Properties.Queries;

public static class GetAllParametersByVersion
{
    public sealed record Query(string Version) : IQuery<List<PropertyResponse>>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository
    ) : IQueryHandler<Query, List<PropertyResponse>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<List<PropertyResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);

            List<DomainError> domainErrors = [];
            domainErrors.CollectErrors(version);

            var validationErrors = CreateValidationErrorIfAny<List<PropertyResponse>>
            (
                (nameof(domainErrors), domainErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var result = await _propertiesRepository.GetAllParametersByVersionAsync(version.Value);
            var persistenceErrors = result.Errors;

            validationErrors = CreateValidationErrorIfAny<List<PropertyResponse>>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var responses = result.Value.Select(PropertyResponse.FromDomain).ToList();
            return Result.Ok(responses);
        }
    }
}
