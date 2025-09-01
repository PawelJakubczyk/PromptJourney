using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.Properties.Queries;

public static class CheckPropertyExistsInVersion
{
    public sealed record Query(ModelVersion Version, PropertyName PropertyName) : IQuery<bool>;

    public sealed class Handler(IPropertiesRepository propertiesRepository) : IQueryHandler<Query, bool>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(query.Version)
                .CollectErrors<PropertyName>(query.PropertyName);

            if (domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<bool>(error);
            }

            return await _propertiesRepository.CheckParameterExistsInVersionAsync(query.Version, query.PropertyName);
        }
    }
}