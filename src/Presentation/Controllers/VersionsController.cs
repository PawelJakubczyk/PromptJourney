using Application.Features.VersionsMaster.Commands;
using Application.Features.VersionsMaster.Queries;
using Application.Features.VersionsMaster.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VersionsController(ISender sender) : ApiController(sender)
{
    // GET api/versions
    [HttpGet]
    [ProducesResponseType<List<VersionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllVersions.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(result.Value);
    }

    // GET api/versions/supported
    [HttpGet("supported")]
    [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupported(CancellationToken cancellationToken)
    {
        var query = new GetAllSuportedVersions.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/versions/{version}
    [HttpGet("{version}")]
    [ProducesResponseType<VersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByVersion(string version, CancellationToken cancellationToken)
    {
        var query = new GetVersionByVersion.Query(version);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/versions/{version}/exists
    [HttpGet("{version}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckExists(string version, CancellationToken cancellationToken)
    {
        var query = new CheckVersionExists.Query(version);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(new { exists = result.Value });
    }

    // POST api/versions
    [HttpPost]
    [ProducesResponseType<VersionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVersionRequest request, CancellationToken cancellationToken)
    {
        var command = new AddVersion.Command(
            request.Version,
            request.Parameter,
            request.ReleaseDate,
            request.Description
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(GetByVersion),
            new { version = result.Value.Version },
            result.Value
        );
    }
}

// Request DTOs
public sealed record CreateVersionRequest(
    string Version,
    string Parameter,
    DateTime? ReleaseDate = null,
    string? Description = null
);