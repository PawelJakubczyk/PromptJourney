using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyProperties;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Properties.Queries;

public class GetAllParametersByVersion
{
    public sealed record Query(ModelVersion Version) : IQuery<List<MidjourneyPropertiesBase>>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
        ) : IQueryHandler<Query, List<MidjourneyPropertiesBase>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;
        public async Task<Result<List<MidjourneyPropertiesBase>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(query.Version);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(query.Version, _versionRepository);

            var validationErrors = CreateValidationErrorIfAny<List<MidjourneyPropertiesBase>>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _propertiesRepository.GetAllParametersByVersionAsync(query.Version);
        }
    }
}
