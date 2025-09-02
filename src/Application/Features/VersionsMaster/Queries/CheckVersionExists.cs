using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using Domain.Errors;
using static Domain.Errors.DomainErrorMessages;
using Domain.ValueObjects;

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

            if (domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<bool>(error);
            }

            return await _versionRepository.CheckVersionExistsInVersionsAsync(query.Version);
        }
    }
}