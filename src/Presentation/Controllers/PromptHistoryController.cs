using Application.Features.PromptHistory.Commands;
using Application.Features.PromptHistory.Queries;
using Application.Features.PromptHistory.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PromptHistoryController(ISender sender) : ApiController(sender)
{
    // GET api/prompthistory
    [HttpGet]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllHistoryRecords.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(result.Value);
    }

    // GET api/prompthistory/last/{count}
    [HttpGet("last/{count:int}")]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLast(int count, CancellationToken cancellationToken)
    {
        var query = new GetLastHistoryRecords.Query(count);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // GET api/prompthistory/daterange?from=2024-01-01&to=2024-12-31
    [HttpGet("daterange")]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        var query = new GetHistoryByDateRange.Query(from, to);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // GET api/prompthistory/keyword/{keyword}
    [HttpGet("keyword/{keyword}")]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByKeyword(string keyword, CancellationToken cancellationToken)
    {
        var query = new GetHistoryRecordsByPromptKeyword.Query(keyword);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // GET api/prompthistory/count
    [HttpGet("count")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecordCount(CancellationToken cancellationToken)
    {
        var query = new CalculateHistoricalRecordCount.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(new { count = result.Value });
    }

    // POST api/prompthistory
    [HttpPost]
    [ProducesResponseType<PromptHistoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPrompt([FromBody] AddPromptRequest request, CancellationToken cancellationToken)
    {
        var command = new AddPromptToHistory.Command(
            request.Prompt,
            request.Version
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(GetRecordCount),
            null,
            result.Value
        );
    }
}

// Request DTOs
public sealed record AddPromptRequest(
    string Prompt,
    string Version
);