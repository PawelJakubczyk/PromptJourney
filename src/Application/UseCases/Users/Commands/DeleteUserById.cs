using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Queries;

public static class GetUserById
{
    public sealed record Query(string UserId) : IQuery<UserResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserResponse>> Handle(Query query, CancellationToken ct)
        {
            var id = UserId.Create(query.UserId);

            var userResult = await _userRepository.GetUserByIdAsync(id.Value, ct);

            var result = WorkflowPipeline
                .Empty()
                .CollectErrors(id)
                .CollectErrors(userResult)
                .MapResult(() => UserResponse.FromDomain(userResult.Value));

            return result;
        }
    }
}
