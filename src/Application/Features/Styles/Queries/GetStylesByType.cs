using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.Styles.Queries;

public static class GetStylesByType
{
    public sealed record Query(string StyleType) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleType = StyleType.Create(query.StyleType);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(styleType)
                .ExecuteIfNoErrors(() => _styleRepository.GetStylesByTypeAsync(styleType.Value, cancellationToken))
                .MapResult(domainList => domainList.Select(StyleResponse.FromDomain).ToList());

            return result;
        }
    }
}