using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Query;
public static class GetUserById
{
    public sealed record Query(string UserId) : IQuery<UserResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserResponse>> Handle(Query query, CancellationToken ct)
        {
            var userId = UserID.Create(query.UserId);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(userId)
                .ExecuteIfNoErrors(() => _userRepository
                    .GetUserByIdAsync(userId.Value, ct))
                .MapResult<MidjourneyUser, UserResponse>
                    (user => UserResponse.FromDomain(user));

            return result;
        }
    }
}
