using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Properties.Responses;
using Domain.Entities;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Properties.Queries;

public static class GetAllProperties
{
    public sealed record Query : IQuery<List<PropertyResponse>>
    {
        public static readonly Query Singleton = new();
    };

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository
    ) : IQueryHandler<Query, List<PropertyResponse>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<List<PropertyResponse>>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .GetAllPropertiesAsync(cancellationToken))
                .MapResult<List<MidjourneyProperty>, List<PropertyResponse>>
                    (propertiesList => [.. propertiesList.Select(PropertyResponse.FromDomain)]);

            return result;
        }
    }
}
