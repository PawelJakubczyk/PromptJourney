using Domain.Entities.MidjourneyPromtHistory;

namespace Application.Features.PromptHistory.Responses;

public sealed record PromptHistoryResponse
(
    Guid HistoryId,
    string Prompt,
    string Version,
    DateTime CreatedOn
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