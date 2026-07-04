using Application.Abstractions;
using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class SoftDeleteUserByEmail
{
    public sealed record Command(string Email) : ICommand<DeleteResponse>;

    public sealed class Handler(
        IUserRepository userRepository,
        ICurrentUser currentUser)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentUser _currentUser = currentUser;
        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken ct)
        {
            var email = Email.Create(command.Email);

            var resultInput = WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(email);

            var userResult = await _userRepository.GetUserByEmailAsync(email.Value, ct);

            var result = await resultInput
                .CollectErrors(userResult)
                .IfUserIsNotCurrentUserOrAdmin(_currentUser, userResult)
                .ExecuteIfNoErrors(() =>
                    _userRepository.SoftDeleteUserByEmailAsync(email.Value, ct))
                .MapResult(() =>
                    DeleteResponse.Success($"User '{email.Value}' was successfully deleted."));

            return result;
        }
    }
}