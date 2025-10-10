using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Properties.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.Features.Properties.Queries;

public static class GetAllProperties
{
    public sealed record Query() : IQuery<List<PropertyResponse>>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : IQueryHandler<Query, List<PropertyResponse>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<List<PropertyResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .GetAllPropertiesAsync(cancellationToken))
                .MapResult<List<MidjourneyProperties>, List<PropertyResponse>>
                    (propertiesList => [.. propertiesList.Select(PropertyResponse.FromDomain)]);

            return result;
        }
    }
}