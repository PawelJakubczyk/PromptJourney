using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Query;

public static class CheckUserNameExists
{
    public sealed record Query(string UserName)
        : IQuery<bool>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken ct)
        {
            var userName = UserName.Create(query.UserName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(userName)
                .ExecuteIfNoErrors(() => _userRepository
                    .CheckUserNameExistsAsync(userName.Value, ct))
                .MapResult<bool>();

            return result;
        }
    }
}
