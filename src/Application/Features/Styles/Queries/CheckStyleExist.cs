using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Queries;

public static class CheckStyleExist
{
    public sealed record Query(string Name) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _styleRepository.CheckStyleExistsAsync(query.Name);
        }
    }
}