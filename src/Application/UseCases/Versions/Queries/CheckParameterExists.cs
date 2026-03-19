using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public static class CheckParameterExists
{
    public sealed record Query(string? Parameter) : IQuery<bool>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, bool>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var parameter = Param.Create(query.Parameter);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(parameter)
                .ExecuteIfNoErrors(() => _versionRepository
                    .CheckParameterExistsAsync(parameter.Value, cancellationToken))
                .MapResult<bool>();

            return result;
        }
    }
}