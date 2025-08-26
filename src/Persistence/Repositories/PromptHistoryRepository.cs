using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
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
            await Validate.Date.CannotBeInFuture(dateFrom);
            await Validate.Date.CannotBeInFuture(dateTo);
            await Validate.Date.Range.MustBeChronological(dateFrom, dateTo);

            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .Where(h => h.CreatedOn >= dateFrom && h.CreatedOn <= dateTo)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync();

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPromptHistory>>($"Failed to get history by date range: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(string keyword)
    {
        try
        {
            await Validate.History.Input.Keyword.MustNotBeNullOrEmpty(keyword);

            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .Where(h => h.Prompt.Contains(keyword))
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync();

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            return Result.Fail<List<MidjourneyPromptHistory>>($"Failed to get history records by prompt keyword: {ex.Message}");
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int count)
    {
        try
        {
            await Validate.History.LimitMustBeGreaterThanZero(count);

            // Get total record count first to validate the count parameter
            var totalRecords = await CalculateHistoricalRecordCountAsync();
            if (totalRecords.IsFailed)
            {
                return Result.Fail<List<MidjourneyPromptHistory>>("Failed to validate record count");
            }

            await Validate.History.CountMustNotExceedHistoricalRecords(count, this);

            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .Take(count)
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
            await Validate.Entity.MustNotBeNull(history, nameof(history));

            // Validate prompt and version using domain validation
            await Validate.History.Input.MustNotBeNullOrEmpty(history.Prompt);
            await Validate.History.Input.MustHaveMaximumLength(history.Prompt);
            await Validate.Version.Input.CannotBeNullOrEmpty(history.Version);

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