using Domain.Entities.MidjourneyPromtHistory;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPromptHistoryRepository
{
    // For Queries
    Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync();
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(Keyword keyword);
    Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int records);
    Task<Result<int>> CalculateHistoricalRecordCountAsync();
    // For Commands
    Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history);
}
