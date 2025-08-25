using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;

namespace Application.Abstractions.IRepository;

public interface IPromptHistoryRepository
{
    // For Queries
    Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync();
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(string keyword);
    Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int keyword);
    Task<Result<int>> CalculateHistoricalRecordCountAsync();
    // For Commands
    Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history);
}
