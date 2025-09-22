using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.PromptHistory.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(string Prompt, string Version) : ICommand<PromptHistoryResponse>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PromptHistoryResponse>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<PromptHistoryResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Create(command.Prompt);
            var version = ModelVersion.Create(command.Version);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(prompt)
                .CollectErrors(version)
                .ExecuteAndMapResultIfNoErrors(
                    () =>
                    {
                        var history = MidjourneyPromptHistory.Create(prompt, version);
                        return _promptHistoryRepository.AddPromptToHistoryAsync(history.Value, cancellationToken);
                    },
                    PromptHistoryResponse.FromDomain
                );

            return result;
        }
    }
}