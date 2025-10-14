using Application.UseCases.PromptHistory.Commands;
using Application.UseCases.PromptHistory.Queries;
using Application.UseCases.PromptHistory.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.ControllersUtilities;

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
        return await Sender
            .Send(GetAllHistoryRecords.Query.Simgletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/prompthistory/last/{count}
    [HttpGet("last/{count:int}")]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLast(int count, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetLastHistoryRecords.Query(count), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/prompthistory/daterange?from=2024-01-01&to=2024-12-31
    [HttpGet("daterange")]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetHistoryByDateRange.Query(from, to), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/prompthistory/keyword/{keyword}
    [HttpGet("keyword/{keyword}")]
    [ProducesResponseType<List<PromptHistoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByKeyword(string keyword, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetHistoryRecordsByPromptKeyword.Query(keyword), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/prompthistory/count
    [HttpGet("count")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecordCount(CancellationToken cancellationToken)
    {
        return await Sender
            .Send(CalculateHistoricalRecordCount.Query.Simgletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { count = payload })))
            .ToActionResultAsync();
    }

    // POST api/prompthistory
    [HttpPost]
    [ProducesResponseType<string>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPrompt([FromBody] AddPromptRequest request, CancellationToken cancellationToken)
    {
        var command = new AddPromptToHistory.Command(
            request.Prompt,
            request.Version
        );

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => {
                if (!string.IsNullOrEmpty(payload)) {
                    return CreatedAtAction(nameof(GetRecordCount), null, new { historyId = payload });
                }

                return NoContent();
            }))
            .ToActionResultAsync();
    }
}

// Request DTOs
public sealed record AddPromptRequest(
    string Prompt,
    string Version
);
