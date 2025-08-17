using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.IRepository;

public interface IPromptHistoryRepository
{
    // For Queries
    Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync();
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
    Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(string keyword);
    Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int keyword);
    // For Commands
    Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history);
}
