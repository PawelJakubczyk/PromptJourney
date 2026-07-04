using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Common.Responses;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Users.Commands;

public static class DeleteUserByEmail
{
    public sealed record Command(string Email) : ICommand<DeleteResponse>;

    public sealed class Handler(IUserRepository userRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken ct)
        {
            var email = Email.Create(command.Email);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(email)
                .IfUserEmailNotExists(email, _userRepository, ct)
                .ExecuteIfNoErrors(() =>
                    _userRepository.DeleteUserByEmailAsync(email.Value, ct))
                .MapResult(() =>
                    DeleteResponse.Success($"User '{email.Value}' was successfully deleted."));

            return result;
        }
    }
}
