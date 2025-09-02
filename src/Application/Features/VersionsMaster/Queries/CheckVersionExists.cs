using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using FluentResults;
using Domain.Errors;
using static Domain.Errors.DomainErrorMessages;
using Domain.ValueObjects;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.VersionsMaster.Queries;

public static class CheckVersionExists
{
    public sealed record Query(ModelVersion Version) : IQuery<bool>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, bool>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(query.Version);

            var validationErrors = CreateValidationErrorIfAny<bool>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _versionRepository.CheckVersionExistsInVersionsAsync(query.Version);
        }
    }
}