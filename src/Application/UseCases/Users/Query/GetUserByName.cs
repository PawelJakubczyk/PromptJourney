using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Query;

public static class GetUserByName
{
    public sealed record Query(string UserName) : IQuery<UserResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserResponse>> Handle(Query query, CancellationToken ct)
        {
            var userName = UserName.Create(query.UserName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(userName)
                .ExecuteIfNoErrors(() => _userRepository
                    .GetUserByNameAsync(userName.Value, ct))
                .MapResult<MidjourneyUser, UserResponse>
                    (user => UserResponse.FromDomain(user));

            return result;
        }
    }
}
