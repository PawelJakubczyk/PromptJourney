using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Properties.Queries;

public class GetAllParametersByVersion
{
    public sealed record Query(string Version) : IQuery<List<PropertyResponse>>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
        ) : IQueryHandler<Query, List<PropertyResponse>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<PropertyResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(version);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository);

            var validationErrors = CreateValidationErrorIfAny<List<PropertyResponse>>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _propertiesRepository.GetAllParametersByVersionAsync(version.Value);

            if (result.IsFailed)
                return Result.Fail<List<PropertyResponse>>(result.Errors);

            var responses = result.Value
                .Select(PropertyResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}
