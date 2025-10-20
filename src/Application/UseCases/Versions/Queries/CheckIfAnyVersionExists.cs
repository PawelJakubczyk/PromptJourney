using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public sealed class CheckIfAnyVersionExists
{
    public sealed record Query() : IQuery<bool>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, bool>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<bool>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _versionRepository
                    .CheckIfAnyVersionExistsAsync(cancellationToken))
                .MapResult<bool>();

            return result;
        }
    }
}