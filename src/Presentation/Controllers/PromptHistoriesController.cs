using Application.UseCases.PromptHistory.Commands;
using Application.UseCases.PromptHistory.Queries;
using Application.UseCases.PromptHistory.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.Utilities;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PromptHistoriesController(ISender sender) : ApiController(sender)
{
    // Queries //

    // GET api/prompthistory
    [HttpGet]
    public async Task<Results<Ok<List<PromptHistoryResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetAll(CancellationToken cancellationToken)
    {
        var histories = await Sender
            .Send(GetAllHistoryRecords.Query.Singletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return histories;
    }

    // GET api/prompthistory/last/{count}
    [HttpGet("last/{count}")]
    public async Task<Results<Ok<List<PromptHistoryResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetLast
    (
        int count, 
        CancellationToken cancellationToken
    )
    {
        var query = new GetLastHistoryRecords.Query(count);

        var histories = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return histories;
    }

    // GET api/prompthistory/daterange?from=2024-01-01&to=2024-12-31
    [HttpGet("date-range")]
    public async Task<Results<Ok<List<PromptHistoryResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByDateRange
    (
        [FromQuery] DateTime from, 
        [FromQuery] DateTime to, 
        CancellationToken cancellationToken
    )
    {
        var query = new GetHistoryByDateRange.Query(from, to);

        var histories = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return histories;
    }

    // GET api/prompthistory/keyword/{keyword}
    [HttpGet("keyword/{keyword}")]
    public async Task<Results<Ok<List<PromptHistoryResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByKeyword
    (
        string keyword, 
        CancellationToken cancellationToken
    )
    {
        var query = new GetHistoryRecordsByPromptKeyword.Query(keyword);

        var histories = await Sender
            .Send(new GetHistoryRecordsByPromptKeyword.Query(keyword), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return histories;
    }

    // GET api/prompthistory/count
    [HttpGet("count")]
    public async Task<Results<Ok<int>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetRecordCount(CancellationToken cancellationToken)
    {
        var hsitories =  await Sender
            .Send(CalculateHistoricalRecordCount.Query.Singletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { count = payload })))
            .ToResultsAsync();

        return hsitories;
    }

    // POST api/prompthistory
    [HttpPost]
    public async Task<Results<Ok<string>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> AddPrompt
    (
        [FromBody] AddPromptRequest request, 
        CancellationToken cancellationToken
    )
    {
        var command = new AddPromptToHistory.Command
        (
            request.Prompt,
            request.Version
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => {
                if (!string.IsNullOrEmpty(payload))
                {
                    return CreatedAtAction(nameof(GetRecordCount), null, new { historyId = payload });
                }

                return NoContent();
            }))
            .ToResultsAsync();

        return result;
    }
}

// Request DTOs
public sealed record AddPromptRequest(
    string Prompt,
    string Version
);
