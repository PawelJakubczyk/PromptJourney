using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.Styles.Queries;

public static class CheckStyleExist
{
    public sealed record Query(string StyleName) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(styleName)
                    .ExecuteIfNoErrors(() => _styleRepository.CheckStyleExistsAsync(styleName.Value, cancellationToken))
                        .MapResult(value => value);


            return result;
        }

    }
}