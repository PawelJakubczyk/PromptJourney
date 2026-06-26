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
    public sealed record Command(string UserName, string Email, string Password)
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
            var userId = UserId.Create();
            var userName = UserName.Create(command.UserName);
            var email = Email.Create(command.Email);

            var hashed = _passwordHasher.Hash(command.Password);
            var passwordHash = PasswordHash.Create(hashed);
            var role = Role.Create("User");

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
                .AggregateErrors(
                    pipeline => pipeline.IfUserNameAlreadyExists(userName, _userRepository, ct),
                    pipeline => pipeline.IfUserEmailAlreadyExists(email, _userRepository, ct)
                )
                .ExecuteIfNoErrors(() => _userRepository.AddUserAsync(user.Value, ct))
                .MapResult(() => UserResponse.FromDomain(user.Value));

            return result;
        }
    }
}
