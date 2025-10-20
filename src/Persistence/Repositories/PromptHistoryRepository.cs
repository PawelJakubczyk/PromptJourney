using Application.Abstractions.IRepository;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Utilities.Constants;
using Utilities.Extensions;

namespace Persistence.Repositories;

public sealed class PromptHistoryRepository(MidjourneyDbContext midjourneyDbContext) : IPromptHistoryRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext = midjourneyDbContext;

    // For Commands
    public async Task<Result<MidjourneyPromptHistory>> AddPromptToHistoryAsync
    (
        MidjourneyPromptHistory history,
        CancellationToken cancellationToken
    )
    {
        await _midjourneyDbContext.MidjourneyPromptHistory.AddAsync(history, cancellationToken);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(history);
    }

    public async Task<Result<MidjourneyPromptHistory>> DeleteHistoryRecordAsync
    (
        Guid historyId,
        CancellationToken cancellationToken
    )
    {
        var historyRecord = await _midjourneyDbContext.MidjourneyPromptHistory
            .FirstOrDefaultAsync(history => history.HistoryId == historyId, cancellationToken);

        if (historyRecord is null) return Result.Fail<MidjourneyPromptHistory>(DomainErrors.HistoryNotFoundError(historyId));

        _midjourneyDbContext.MidjourneyPromptHistory.Remove(historyRecord);
        await _midjourneyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok(historyRecord);
    }

    // For Queries
    public async Task<Result<int>> CalculateHistoricalRecordCountAsync(CancellationToken cancellationToken)
    {
        var count = await _midjourneyDbContext.MidjourneyPromptHistory.CountAsync(cancellationToken);
        return Result.Ok(count);
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetAllHistoryRecordsAsync(CancellationToken cancellationToken)
    {
        var records = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .OrderByDescending(history => history.CreatedOn)
            .ToListAsync(cancellationToken);

        return Result.Ok(records);
    }

    public async Task<Result<MidjourneyPromptHistory>> GetHistoryRecordByIdAsync
    (
        Guid historyId,
        CancellationToken cancellationToken
    )
    {
        var record = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .FirstOrDefaultAsync(history => history.HistoryId == historyId, cancellationToken);

        if (record is null)
        {
            var notFoundError = ErrorBuilder.New()
                .WithLayer<DomainLayer>()
                .WithMessage($"History record with ID {historyId} not found")
                .WithErrorCode(StatusCodes.Status404NotFound)
                .Build();

            return Result.Fail<MidjourneyPromptHistory>(notFoundError);
        }

        return Result.Ok(record);
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryByDateRangeAsync
    (
        DateTime dateFrom, 
        DateTime dateTo, 
        CancellationToken cancellationToken
    )
    {
        var records = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .Where(history => history.CreatedOn >= dateFrom && history.CreatedOn <= dateTo)
            .OrderByDescending(history => history.CreatedOn)
            .ToListAsync(cancellationToken);

        return Result.Ok(records);
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByPromptKeywordAsync
    (
        Keyword keyword, 
        CancellationToken cancellationToken
    )
    {
        var pattern = keyword.ToString();

        var records = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .Where(history => EF.Functions.Like(history.Prompt.Value, pattern))
            .OrderByDescending(history => history.CreatedOn)
            .ToListAsync(cancellationToken);

        return Result.Ok(records);
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetHistoryRecordsByVersionAsync
    (
        ModelVersion version, 
        CancellationToken cancellationToken
    )
    {
        var records = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .Where(history => history.Version == version)
            .OrderByDescending(history => history.CreatedOn)
            .ToListAsync(cancellationToken);

        return Result.Ok(records);
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetLastHistoryRecordsAsync
    (
        int records, 
        CancellationToken cancellationToken
    )
    {
        var list = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .OrderByDescending(history => history.CreatedOn)
            .Take(records)
            .ToListAsync(cancellationToken);

        return Result.Ok(list);
    }

    public async Task<Result<List<MidjourneyPromptHistory>>> GetPaginatedHistoryRecordsAsync
    (
        int pageSize, 
        int pageNumber, 
        CancellationToken cancellationToken
    )
    {
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than zero", nameof(pageSize));
        if (pageNumber <= 0)
            throw new ArgumentException("Page number must be greater than zero", nameof(pageNumber));

        var list = await _midjourneyDbContext.MidjourneyPromptHistory
            .Include(history => history.MidjourneyVersion)
            .Include(history => history.MidjourneyStyles)
            .OrderByDescending(history => history.CreatedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Result.Ok(list);
    }
}