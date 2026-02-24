using Domain.Entities;

namespace Application.UseCases.PromptHistory.Responses;

public sealed record PromptHistoryResponse
(
    Guid HistoryId,
    string Prompt,
    string Version,
    DateTimeOffset CreatedOn
)
{
    public static PromptHistoryResponse FromDomain(MidjourneyPromptHistory history) =>
        new(
            history.HistoryId,
            history.Prompt.Value,
            history.Version.Value,
            history.CreatedOn
        );
}