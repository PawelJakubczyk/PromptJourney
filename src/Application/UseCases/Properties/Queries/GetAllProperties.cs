using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Properties.Responses;
using Domain.Entities;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Properties.Queries;

public static class GetAllProperties
{
    public sealed record Query : IQuery<List<PropertyQueryResponse>>
    {
        public static readonly Query Singleton = new();
    };

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository
    ) : IQueryHandler<Query, List<PropertyQueryResponse>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<List<PropertyQueryResponse>>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .GetAllPropertiesAsync(cancellationToken))
                .MapResult<List<MidjourneyProperties>, List<PropertyQueryResponse>>
                    (propertiesList => [.. propertiesList.Select(PropertyQueryResponse.FromDomain)]);

            return result;
        }
    }
}
