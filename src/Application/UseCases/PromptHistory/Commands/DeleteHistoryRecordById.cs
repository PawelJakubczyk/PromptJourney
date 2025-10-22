using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Common.Responses;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Commands;

public sealed class DeleteHistoryRecordById
{
    public sealed record Command(Guid Id) : ICommand<DeleteResponse>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .DeleteHistoryRecordByIdAsync(command.Id, cancellationToken))
                .MapResult(() => DeleteResponse.Success($"Style with Id: '{command.Id}' was successfully deleted."));

            return result;
        }
    }
}