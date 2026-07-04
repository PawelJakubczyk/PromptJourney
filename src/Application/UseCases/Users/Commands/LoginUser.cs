using Application.Abstractions;
using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class LoginUser
{
    public sealed record Command(string Email, string Password)
        : ICommand<LoginResponse>;

    public sealed class Handler
    (
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    ) : ICommandHandler<Command, LoginResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<LoginResponse>> Handle(Command command, CancellationToken ct)
        {
            var email = Email.Create(command.Email);
            var userResult = await _userRepository.GetUserByEmailAsync(email.Value, ct);

            var result = WorkflowPipeline
                .Empty()
                .CollectErrors(email)
                .CollectErrors(userResult)
                .ExecuteIfNoErrors(() =>
                {
                    var user = userResult.Value;

                    if (!_passwordHasher.Verify(user.PasswordHash.Value, command.Password))
                        return Result.Fail<LoginResponse>("Invalid credentials.");

                    var token = _tokenService.GenerateAccessToken(user);
                    return Result.Ok(LoginResponse.FromDomain(token, UserResponse.FromDomain(user)));
                })
                .MapResult<LoginResponse>();

            return result;
        }
    }
}
