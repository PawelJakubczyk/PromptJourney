using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyProperties;
using FluentResults;

namespace Application.Features.Properties.Queries;

public class GetAllPropertiesByVersion
{
    public record Query(string Version) : IQuery<List<MidjourneyPropertiesBase>>;

    public class GetAllPropertiesByVersionMasterQueryHandler(IVersionRepository versionRepository) 
        : IQueryHandler<Query, List<MidjourneyPropertiesBase>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<MidjourneyPropertiesBase>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _versionRepository.GetAllParametersByVersionAsync(query.Version);
        }
    }
}