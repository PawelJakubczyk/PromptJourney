using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyVersions;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.VersionsMaster.Queries;

public static class GetVersionByVersion
{
    public sealed record Query(ModelVersion Version) : IQuery<MidjourneyVersion>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, MidjourneyVersion>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersion>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(query.Version, _versionRepository);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(query.Version);

            var validationErrors = CreateValidationErrorIfAny<MidjourneyVersion>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _versionRepository.GetMasterVersionByVersionAsync(query.Version);
        }
    }
}