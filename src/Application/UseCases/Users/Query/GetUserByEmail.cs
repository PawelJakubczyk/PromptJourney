using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Queries;

public static class GetUserByEmail
{
    public sealed record Query(string Email) : IQuery<UserResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : IQueryHandler<Query, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserResponse>> Handle(Query query, CancellationToken ct)
        {
            var email = Email.Create(query.Email);

            var userResult = await _userRepository.GetUserByEmailAsync(email.Value, ct);

            var result = WorkflowPipeline
                .Empty()
                .CollectErrors(email)
                .CollectErrors(userResult)
                .MapResult(() => UserResponse.FromDomain(userResult.Value));

            return result;
        }
    }
}
