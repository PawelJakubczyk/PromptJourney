using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class UpdateUserEmail
{
    public sealed record Command(string UserId, string NewEmail)
        : ICommand<UserResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : ICommandHandler<Command, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserResponse>> Handle(Command command, CancellationToken ct)
        {
            var id = UserId.Create(command.UserId);
            var email = Email.Create(command.NewEmail);

            var userResult = await _userRepository.GetUserByIdAsync(id.Value, ct);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(id)
                .CollectErrors(email)
                .ExecuteIfNoErrors(() =>
                    _userRepository.UpdateUserEmailById(id.Value, email.Value, ct))
                .MapResult(() => UserResponse.FromDomain(userResult.Value));

            return result;
        }
    }
}
