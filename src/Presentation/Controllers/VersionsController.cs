using Application.UseCases.Versions.Commands;
using Application.UseCases.Versions.Queries;
using Application.UseCases.Versions.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.Utilities;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VersionsController(ISender sender) : ApiController(sender)
{
    // GET api/versions
    [HttpGet]
    public async Task<Results<Ok<List<VersionResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllVersions.Query();

        var versions = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return versions;
    }

    // GET api/versions/supported
    [HttpGet("supported")]
    public async Task<Results<Ok<List<string>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetSupported(CancellationToken cancellationToken)
    {
        var query = new GetAllSuportedVersions.Query();

        var versions = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return versions;
    }

    // GET api/versions/{version}
    [HttpGet("{version}")]
    public async Task<Results<Ok<VersionResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByVersion(string version, CancellationToken cancellationToken)
    {
        var query = new GetVersion.Query(version);

        var versionInfo = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return versionInfo;
    }

    // GET api/versions/{version}/exists
    [HttpGet("{version}/exists")]
    public async Task<Results<Ok<bool>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> CheckExists(string version, CancellationToken cancellationToken)
    {
        var query = new CheckVersionExists.Query(version);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToResultsAsync();

        return exist;
    }

    // POST api/versions
    [HttpPost]
    public async Task<Results<Ok<string>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> Create([FromBody] CreateVersionRequest request, CancellationToken cancellationToken)
    {
        var command = new AddVersion.Command
        (
            request.Version,
            request.Parameter,
            request.ReleaseDate,
            request.Description
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => {
                if (!string.IsNullOrEmpty(payload)) {
                    return CreatedAtAction(nameof(GetByVersion), new { version = payload }, new { version = payload });
                }

                return NoContent();
            }))
            .ToResultsAsync();

        return result;
    }
}

// Request DTOs
public sealed record CreateVersionRequest(
    string Version,
    string Parameter,
    DateTime? ReleaseDate = null,
    string? Description = null
);
