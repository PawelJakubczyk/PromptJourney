using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class DeleteUserById
{
    public sealed record Command(string UserId) : ICommand<DeleteResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken ct)
        {
            var userId = UserID.Create(command.UserId);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(userId)
                .IfUserIdNotExists(userId, _userRepository, ct)
                .ExecuteIfNoErrors(() =>
                    _userRepository.DeleteUserByIdAsync(userId.Value, ct))
                .MapResult(() =>
                    DeleteResponse.Success($"User '{userId.Value}' was successfully deleted."));
            return result;
        }
    }
}
