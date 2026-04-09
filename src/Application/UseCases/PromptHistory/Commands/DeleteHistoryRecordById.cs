using Application.Abstractions;
using Application.Abstractions.IRepository;
using Utilities.Results;
using Application.UseCases.Common.Responses;
using Utilities.Workflows;
using Domain.ValueObjects;

namespace Application.UseCases.PromptHistory.Commands;

public sealed class DeleteHistoryRecordById
{
    public sealed record Command(string? Id) : ICommand<DeleteResponse>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var historyId = HistoryID.Create(command.Id);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(historyId)
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .DeleteHistoryRecordByIdAsync(historyId.Value, cancellationToken))
                .MapResult(() => DeleteResponse.Success($"History record with Id: '{historyId.Value}' was successfully deleted."));

            return result;
        }
    }
}