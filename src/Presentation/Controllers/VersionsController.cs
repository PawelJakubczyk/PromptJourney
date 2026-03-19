using Application.UseCases.Common.Responses;
using Application.UseCases.Versions.Commands;
using Application.UseCases.Versions.Queries;
using Application.UseCases.Versions.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.Pipeline;

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
            .Send(GetAllVersions.Query.Singleton, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<List<VersionResponse>, BadRequest<ProblemDetails>>(HttpContext);

        return versions;
    }

    // GET api/versions/supported
    [HttpGet("supported")]
    public async Task<Results<Ok<List<string>>, BadRequest<ProblemDetails>>> GetSupported(CancellationToken cancellationToken)
    {
        var versions = await Sender
            .Send(GetAllSupportedVersions.Query.Singleton, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<List<string>, BadRequest<ProblemDetails>>(HttpContext);

        return versions;
    }

    // GET api/versions/no-empty
    [HttpGet("no-empty")]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckAnyExists(CancellationToken cancellationToken)
    {
        var exists = await Sender
            .Send(CheckIfAnyVersionExists.Query.Singleton, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exists;
    }

    // ✅ GET api/versions/latest
    [HttpGet("latest")]
    public async Task<Results<Ok<VersionResponse>, NotFound<ProblemDetails>>> GetLatest(
        CancellationToken cancellationToken)
    {
        var latestVersion = await Sender
            .Send(GetLatestVersion.Query.Singleton, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<VersionResponse, NotFound<ProblemDetails>>(HttpContext);

        return latestVersion;
    }

    // GET api/versions/{version}
    [HttpGet("{version}")]
    public async Task<Results<Ok<VersionResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByVersion(
        string version, 
        CancellationToken cancellationToken)
    {
        var query = new GetVersion.Query(version);

        var versionInfo = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<VersionResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

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
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exist;
    }

    // GET api/versions/{parameter}/parrameterexists
    [HttpGet("{parameter}/parrameterexists")]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckParameterExists(string parameter, CancellationToken cancellationToken)
    {
        var query = new CheckParameterExists.Query(parameter);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exist;
    }

    // POST api/versions
    [HttpPost]
    public async Task<Results<Created<VersionResponse>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> Create
    (
        [FromBody] CreateVersionRequest request, 
        CancellationToken cancellationToken
    )
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
            .ElsePrepareCreateResponse()
            .ToResultsCreatedAsync<VersionResponse, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>
            (
                locationFactory: version => $"/api/versions/{version?.Version}",
                httpContext: HttpContext
            );

        return result;
    }

    // DELETE api/versions/{version}
    [HttpDelete("{version}")]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> Delete
    (
        string version,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteVersion.Command(version);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<DeleteResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }
}

// Request DTOs
public sealed record CreateVersionRequest(
    string Version,
    string Parameter,
    string ReleaseDate,
    string? Description = null
);
