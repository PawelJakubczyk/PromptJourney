using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.PromptHistory.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

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

            var promptHistory = MidjourneyPromptHistory.Create(prompt, version);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(promptHistory)
                .Validate(pipeline => pipeline
                    .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                    .IfVersionNotInSuportedVersions(version.Value, _versionRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory.Value, cancellationToken))
                .MapResult(PromptHistoryResponse.FromDomain);

            return result;
        }
    }
}