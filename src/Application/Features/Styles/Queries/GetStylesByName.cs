using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Queries;

public static class GetStylesByName
{
    public sealed record Query(string Name) : IQuery<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Style.Input.CannotBeNullOrEmpty(query.Name);
            await Validate.Style.Input.MustHaveMaximumLenght(query.Name);
            await Validate.Style.MustExists(query.Name, _styleRepository);

            return await _styleRepository.GetStyleByNameAsync(query.Name);
        }
    }
}