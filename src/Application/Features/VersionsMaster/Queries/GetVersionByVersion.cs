using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyVersions;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

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

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<MidjourneyVersion> (error);
            }

            return await _versionRepository.GetMasterVersionByVersionAsync(query.Version);
        }
    }
}