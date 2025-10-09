using Application.Features.Versions.Commands;
using Application.Features.Versions.Queries;
using Application.Features.Versions.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.ControllersUtilities;

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
        return await Sender
            .Send(new GetAllVersions.Query(), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/versions/supported
    [HttpGet("supported")]
    [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupported(CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetAllSuportedVersions.Query(), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/versions/{version}
    [HttpGet("{version}")]
    [ProducesResponseType<VersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByVersion(string version, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetVersion.Query(version), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/versions/{version}/exists
    [HttpGet("{version}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckExists(string version, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckVersionExists.Query(version), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToActionResultAsync();
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

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload =>
            {
                if (payload is not null)
                {
                    return CreatedAtAction(nameof(GetByVersion), new { version = ((VersionResponse)payload).Version }, payload);
                }

                return NoContent();
            }))
            .ToActionResultAsync();
    }
}

// Request DTOs
public sealed record CreateVersionRequest(
    string Version,
    string Parameter,
    DateTime? ReleaseDate = null,
    string? Description = null
);