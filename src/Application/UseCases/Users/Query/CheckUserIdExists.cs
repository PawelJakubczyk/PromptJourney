using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Query;

public static class CheckUserIdExists
{
    public sealed record Query(string UserId)
        : IQuery<bool>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken ct)
        {
            var userId = UserID.Create(query.UserId);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(userId)
                .ExecuteIfNoErrors(() => _userRepository
                    .CheckUserIdExistsAsync(userId.Value, ct))
                .MapResult<bool>();

            return result;
        }
    }
}
