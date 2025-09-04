using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.VersionsMaster.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.VersionsMaster.Queries;

public static class GetVersionByVersion
{
    public sealed record Query(string Version) : IQuery<VersionResponse>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(version);

            var validationErrors = CreateValidationErrorIfAny<VersionResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _versionRepository.GetMasterVersionByVersionAsync(version.Value);

            if (result.IsFailed)
                return Result.Fail<VersionResponse>(result.Errors);

            var response = VersionResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}