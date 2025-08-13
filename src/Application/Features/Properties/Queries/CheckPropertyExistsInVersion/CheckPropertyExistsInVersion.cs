using Application.Abstractions;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Queries.CheckPropertyExistsInVersion;

public interface IQuery<TValue> : IRequest<Result<TValue>>;

public interface IQueryHandler<TQuery, TValue> : IRequestHandler<TQuery, Result<TValue>> where TQuery : IQuery<TValue>;

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