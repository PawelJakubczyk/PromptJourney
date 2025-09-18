using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class PromptHistoryRepository : IPromptHistoryRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public PromptHistoryRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    // For Queries
    public async Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync()
    {
        try
        {
            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync();

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPromptHistory>>($"Failed to get all history records: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync();

            var filteredRecords = historyRecords
                .Where(h => h.CreatedOn >= dateFrom && h.CreatedOn <= dateTo)
                .ToList();

            return Result.Ok(filteredRecords);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPromptHistory>>($"Failed to get history by date range: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(Keyword keyword)
    {
        try
        {
            var allHistoryRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync();

            var filteredRecords = allHistoryRecords
                .Where(h => h.Prompt.Value.Contains(keyword.Value, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Result.Ok(filteredRecords);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPromptHistory>>($"Failed to get history records by prompt keyword: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int records)
    {
        try
        {
            var totalRecords = await CalculateHistoricalRecordCountAsync();
            if (totalRecords.IsFailed)
            {
                return Result.Fail<List<MidjourneyPromptHistory>>("Failed to validate record count");
            }

            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .Take(records)
                .ToListAsync();

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPromptHistory>>($"Failed to get last history records: {ex.Message}");
        }
    }

    public async Task<Result<int>> CalculateHistoricalRecordCountAsync()
    {
        try
        {
            var count = await _midjourneyDbContext.MidjourneyPromptHistory
                .CountAsync();

            return Result.Ok(count);
        }
        catch (Exception ex)
        {
            return Result.Fail<int>($"Failed to calculate historical record count: {ex.Message}");
        }
    }

    // For Commands
    public async Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history)
    {
        try
        {
            await _midjourneyDbContext.MidjourneyPromptHistory.AddAsync(history);
            await _midjourneyDbContext.SaveChangesAsync();

            return Result.Ok(history);
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyPromptHistory>($"Failed to add prompt to history: {ex.Message}");
        }
    }
}