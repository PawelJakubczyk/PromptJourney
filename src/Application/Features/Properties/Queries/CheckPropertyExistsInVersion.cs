using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.Properties.Queries;

public static class CheckPropertyExistsInVersion
{
    public sealed record Query(string Version, string PropertyName) : IQuery<bool>;

    public sealed class Handler(IPropertiesRopository propertiesRepository) : IQueryHandler<Query, bool>
    {
        private readonly IPropertiesRopository _propertiesRepository = propertiesRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _propertiesRepository.CheckParameterExistsInVersionAsync(query.Version, query.PropertyName);
        }
    }
}