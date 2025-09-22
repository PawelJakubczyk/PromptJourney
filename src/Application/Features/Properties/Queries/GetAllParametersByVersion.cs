using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Properties.Responses;
using Domain.ValueObjects;
using FluentResults;

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

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(version)
                .ExecuteAndMapResultIfNoErrors(
                    () => _propertiesRepository.GetAllParametersByVersionAsync(version.Value, cancellationToken),
                    domainList => domainList.Select(PropertyResponse.FromDomain).ToList()
                );

            return result;
        }
    }
}
