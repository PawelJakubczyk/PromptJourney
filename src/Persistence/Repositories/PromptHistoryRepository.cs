using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using static Persistence.Repositories.Helper.RepositoryHelper;

namespace Persistence.Repositories;

public sealed class PromptHistoryRepository(MidjourneyDbContext midjourneyDbContext) : IPromptHistoryRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext = midjourneyDbContext;

    // For Queries
    public Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(history => history.MidjourneyVersion)
                .Include(history => history.MidjourneyStyles)
                .OrderByDescending(history => history.CreatedOn)
                .ToListAsync(cancellationToken);
        }, "Failed to get all history records", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(history => history.MidjourneyVersion)
                .Include(history => history.MidjourneyStyles)
                .Where(history => history.CreatedOn >= dateFrom && history.CreatedOn <= dateTo)
                .OrderByDescending(history => history.CreatedOn)
                .ToListAsync(cancellationToken);
        }, "Failed to get history by date range", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync(Keyword keyword, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            var pattern = keyword.ToString();

            return await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(history => history.MidjourneyVersion)
                .Include(history => history.MidjourneyStyles)
                .Where(history => EF.Functions.Like(history.Prompt.Value, pattern))
                .OrderByDescending(history => history.CreatedOn)
                .ToListAsync(cancellationToken);
        }, "Failed to get history records by prompt keyword", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync(int records, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyPromptHistory
                .Include(history => history.MidjourneyVersion)
                .Include(history => history.MidjourneyStyles)
                .OrderByDescending(history => history.CreatedOn)
                .Take(records)
                .ToListAsync(cancellationToken);
        }, "Failed to get last history records", StatusCodes.Status500InternalServerError);
    }

    public Task<Result<int>> CalculateHistoricalRecordCountAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            return await _midjourneyDbContext.MidjourneyPromptHistory.CountAsync(cancellationToken);
        }, "Failed to calculate historical record count", StatusCodes.Status500InternalServerError);
    }

    // For Commands
    public Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync(MidjourneyPromptHistory history, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async () =>
        {
            await _midjourneyDbContext.MidjourneyPromptHistory.AddAsync(history, cancellationToken);
            await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
            return history;
        }, "Failed to add prompt to history", StatusCodes.Status500InternalServerError);
    }
}
