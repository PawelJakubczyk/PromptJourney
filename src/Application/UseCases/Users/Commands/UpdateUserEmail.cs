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

public static class UpdateUserEmail
{
    public sealed record Command(string UserId, string NewEmail)
        : ICommand<UserResponse>;

    public sealed class Handler(IUserRepository userRepository, ICurrentUser currentUser)
        : ICommandHandler<Command, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Result<UserResponse>> Handle(Command command, CancellationToken ct)
        {
            var id = UserID.Create(command.UserId);
            var email = Email.Create(command.NewEmail);

            var resultInput = WorkflowPipeline
                .EmptyAsync()
                .AggregateErrors(
                    pipeline => pipeline.CollectErrors(id),
                    pipeline => pipeline.CollectErrors(email));

            if (resultInput.Result.BreakOnError)
            {
                return await resultInput.MapResult<UserResponse>();
            }

            var userResult = await _userRepository.GetUserByIdAsync(id.Value, ct);

            var result = await resultInput
                .IfUserIsNotCurrentUserOrAdmin(_currentUser, userResult)
                .AggregateErrors(
                    pipeline => pipeline.IfUserIdNotExists(id, _userRepository, ct),
                    pipeline => pipeline.IfUserEmailAlreadyExists(email, _userRepository, ct))
                .ExecuteIfNoErrors(() =>
                    _userRepository.UpdateUserEmailByIdAsync(id.Value, email.Value, ct))
                .MapResult(() => UserResponse.FromDomain(userResult.Value));

            return result;
        }
    }
}
