using Application.Abstractions;
using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Users.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class RegisterUser
{
    public sealed record Command(string UserName, string Email, string Password, string Role = "User")
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
            var userId = UserID.Create();
            var userName = UserName.Create(command.UserName);
            var email = Email.Create(command.Email);

            var hashed = _passwordHasher.Hash(command.Password);
            var passwordHash = PasswordHash.Create(hashed);
            var role = Role.Create(command.Role);

            var user = MidjourneyUser.Register(
                userId,
                userName,
                email,
                passwordHash,
                role
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(user)
                .IfUserAlreadyExists(email, userName, _userRepository, ct)
                .ExecuteIfNoErrors(() => _userRepository.RegisterUserAsync(user.Value, ct))
                .MapResult(() => UserResponse.FromDomain(user.Value));

            return result;
        }
    }
}
