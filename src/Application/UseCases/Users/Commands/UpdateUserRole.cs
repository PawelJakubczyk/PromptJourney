using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Users.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class UpdateUserRole
{
    public sealed record Command(string UserId, string NewRole)
        : ICommand<UserResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : ICommandHandler<Command, UserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserResponse>> Handle(Command command, CancellationToken ct)
        {
            var id = UserID.Create(command.UserId);
            var role = Role.Create(command.NewRole);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .AggregateErrors(
                    pipeline => pipeline.CollectErrors(id),
                    pipeline => pipeline.CollectErrors(role))
                .IfUserIdNotExists(id, _userRepository, ct)
                .ExecuteIfNoErrors(() =>
                    _userRepository.UpdateUserRoleByIdAsync(id.Value, role.Value, ct))
                .MapResult<MidjourneyUser, UserResponse>(UserResponse.FromDomain);

            return result;
        }
    }
}