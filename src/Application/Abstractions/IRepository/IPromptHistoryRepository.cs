using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPromptHistoryRepository
{
    // For Queries
    Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync(CancellationToken cancellationToken);
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(Keyword keyword, CancellationToken cancellationToken);
    Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int records, CancellationToken cancellationToken);
    Task<Result<int>> CalculateHistoricalRecordCountAsync(CancellationToken cancellationToken);
    // For Commands
    Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history, CancellationToken cancellationToken);
}
