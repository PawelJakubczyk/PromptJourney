using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Utilities.Constants;
using Utilities.Errors;

public sealed class PromptHistoryRepository : IPromptHistoryRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public PromptHistoryRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    // For Queries
    public async Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync(cancellationToken);

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get all history records: {ex.Message}");
            return Result.Fail<List<MidjourneyPromptHistory>>(error);
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
    {
        try
        {
            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .Where(h => h.CreatedOn >= dateFrom && h.CreatedOn <= dateTo)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync(cancellationToken);

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get history by date range: {ex.Message}");
            return Result.Fail<List<MidjourneyPromptHistory>>(error);
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        try
        {
            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .Where(h => h.Prompt.Value.Contains(keyword.Value, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync(cancellationToken);

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get history records by prompt keyword: {ex.Message}");
            return Result.Fail<List<MidjourneyPromptHistory>>(error);
        }
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int records, CancellationToken cancellationToken)
    {
        try
        {
            var totalRecords = await CalculateHistoricalRecordCountAsync(cancellationToken);
            if (totalRecords.IsFailed)
                return Result.Fail<List<MidjourneyPromptHistory>>(totalRecords.Errors);

            var historyRecords = await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(h => h.VersionMaster)
                .Include(h => h.MidjourneyStyles)
                .OrderByDescending(h => h.CreatedOn)
                .Take(records)
                .ToListAsync(cancellationToken);

            return Result.Ok(historyRecords);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to get last history records: {ex.Message}");
            return Result.Fail<List<MidjourneyPromptHistory>>(error);
        }
    }

    public async Task<Result<int>> CalculateHistoricalRecordCountAsync(CancellationToken cancellationToken)
    {
        try
        {
            var count = await _midjourneyDbContext.MidjourneyPromptHistory.CountAsync(cancellationToken);
            return Result.Ok(count);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to calculate historical record count: {ex.Message}");
            return Result.Fail<int>(error);
        }
    }

    // For Commands
    public async Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history, CancellationToken cancellationToken)
    {
        try
        {
            await _midjourneyDbContext.MidjourneyPromptHistory.AddAsync(history, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(history);
        }
        catch (Exception ex)
        {
            var error = new Error<PersistenceLayer>($"Failed to add prompt to history: {ex.Message}");
            return Result.Fail<MidjourneyPromptHistory>(error);
        }
    }
}
