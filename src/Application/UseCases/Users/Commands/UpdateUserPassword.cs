using Application.Abstractions;
using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class UpdateUserPassword
{
    public sealed record Command(string Email, string NewPassword)
        : ICommand<UserResponse>;

    public sealed class Handler
    (
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ICurrentUser currentUser
    ) : ICommandHandler<Command, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<UserResponse>> Handle(Command command, CancellationToken ct)
        {
            var email = Email.Create(command.Email);
            var hashed = _passwordHasher.Hash(command.NewPassword);
            var passwordHash = PasswordHash.Create(hashed);

            var resultImput = WorkflowPipeline
                .EmptyAsync()
                .AggregateErrors(
                    pipeline => pipeline.CollectErrors(email),
                    pipeline => pipeline.CollectErrors(passwordHash));

            if (resultImput.Result.BreakOnError)
            {
                return await resultImput.MapResult<UserResponse>();
            }

            var userResult = await _userRepository.GetUserByEmailAsync(email.Value, ct);

            var result = await resultImput
                .CollectErrors(userResult)
                .IfUserIsNotCurrentUserOrAdmin(_currentUser, userResult)
                .ExecuteIfNoErrors(async () =>
                {
                    await _userRepository.UpdateUserHashedPasswordAsync(email.Value, passwordHash.Value, ct);

                    var user = await _userRepository.GetUserByEmailAsync(email.Value, ct);
                    return Result.Ok(UserResponse.FromDomain(user.Value));
                })
                .MapResult<UserResponse>();

            return result;
        }
    }
}
