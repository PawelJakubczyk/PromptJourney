using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.Properties.Queries;

public static class CheckPropertyExistsInVersion
{
    public sealed record Query(string Version, string PropertyName) : IQuery<bool>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository
    ) : IQueryHandler<Query, bool>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);
            var propertyName = PropertyName.Create(query.PropertyName);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(version)
                .CollectErrors(propertyName)
                .ExecuteAndMapResultIfNoErrors(
                    () => _propertiesRepository.CheckParameterExistsInVersionAsync(version.Value, propertyName.Value, cancellationToken),
                    value => value
                );

            return result;
        }

    }
}