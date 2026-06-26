using Application.Abstractions;
using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class UpdateUserPassword
{
    public sealed record Command(string UserId, string NewPassword)
        : ICommand<UserResponse>;

    public sealed class Handler
    (
        IUserRepository userRepository,
        IPasswordHasher passwordHasher
    ) : ICommandHandler<Command, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<UserResponse>> Handle(Command command, CancellationToken ct)
        {
            var id = UserId.Create(command.UserId);

            var hashed = _passwordHasher.Hash(command.NewPassword);
            var passwordHash = PasswordHash.Create(hashed);

            var userResult = await _userRepository.GetUserByIdAsync(id.Value, ct);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(id)
                .CollectErrors(passwordHash)
                .CollectErrors(userResult)
                .ExecuteIfNoErrors(() =>
                    _userRepository.UpdateUserHashedPasswordById(id.Value, hashed, ct))
                .MapResult(() => UserResponse.FromDomain(userResult.Value));

            return result;
        }
    }
}
