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
    public async Task<Results<Ok<List<VersionResponse>>, BadRequest<ProblemDetails>>> GetAll(CancellationToken cancellationToken)
    {
        var versions = await Sender
            .Send(GetAllVersions.Query.Singletone, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsSimpleOkAsync();

        return versions;
    }

    // GET api/versions/supported
    [HttpGet("supported")]
    public async Task<Results<Ok<List<string>>, BadRequest<ProblemDetails>>> GetSupported(CancellationToken cancellationToken)
    {
        var versions = await Sender
            .Send(GetAllSuportedVersions.Query.Singletone, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsSimpleOkAsync();

        return versions;
    }

    // GET api/versions/{version}
    [HttpGet("{version}")]
    public async Task<Results<Ok<VersionResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByVersion(string version, CancellationToken cancellationToken)
    {
        var query = new GetVersion.Query(version);

        var versionInfo = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync();

        return versionInfo;
    }

    // GET api/versions/{version}/exists
    [HttpGet("{version}/exists")]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckExists(string version, CancellationToken cancellationToken)
    {
        var query = new CheckVersionExists.Query(version);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(new { exists = payload }))
            .ToResultsSimpleOkAsync();

        return exist;
    }

    // POST api/versions
    [HttpPost]
    public async Task<Results<Created<string>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> Create([FromBody] CreateVersionRequest request, CancellationToken cancellationToken)
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
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareCreateResponse(payload => 
                CreatedAtAction
                (
                    nameof(GetByVersion), 
                    new { version = payload }, 
                    new { version = payload }
                )
            )
            .ToResultsCreatedAsync();

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
