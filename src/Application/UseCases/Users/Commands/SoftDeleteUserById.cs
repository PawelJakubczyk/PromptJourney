using Application.Abstractions;
using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Application.UseCases.Users.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class SoftDeleteUserById
{
    public sealed record Command(string UserId) : ICommand<DeleteResponse>;

    public sealed class Handler(IUserRepository userRepository, ICurrentUser _currentUser)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentUser _currentUser = _currentUser;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken ct)
        {
            var userId = UserID.Create(command.UserId);

            var resultInput = WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(userId);

            if (resultInput.Result.BreakOnError)
            {
                return await resultInput.MapResult<DeleteResponse>();
            }

            var userResult = await _userRepository.GetUserByIdAsync(userId.Value, ct);

            var result = await resultInput
                .IfUserIdNotExists(userId, _userRepository, ct)
                .IfUserIsNotCurrentUserOrAdmin(_currentUser, userResult)
                .ExecuteIfNoErrors(() =>
                    _userRepository.SoftDeleteUserByIdAsync(userId.Value, ct))
                .MapResult(() =>
                    DeleteResponse.Success($"User '{userId.Value}' was successfully deleted."));
            return result;
        }
    }
}
