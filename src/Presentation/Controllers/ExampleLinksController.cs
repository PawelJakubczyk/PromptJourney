using Application.UseCases.Common.Responses;
using Application.UseCases.ExampleLinks.Commands;
using Application.UseCases.ExampleLinks.Queries;
using Application.UseCases.ExampleLinks.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.ControllersUtilities;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ExampleLinksController(ISender sender) : ApiController(sender)
{
    // GET api/examplelinks
    [HttpGet]
    public async Task<Results<Ok<List<ExampleLinkResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetAll(CancellationToken cancellationToken)
    {
        var styles = await Sender
            .Send(GetAllExampleLinks.Query.Singletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return styles;
    }

    // GET api/examplelinks/style/{styleName}
    [HttpGet("style/{styleName}")]
    public async Task<Results<Ok<List<ExampleLinkResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByStyle(string styleName, CancellationToken cancellationToken)
    {
        var query = new GetExampleLinksByStyle.Query(styleName);

        var styles = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return styles;
    }

    // GET api/examplelinks/style/{styleName}/version/{version}
    [HttpGet("style/{styleName}/version/{version}")]
    public async Task<Results<Ok<List<ExampleLinkResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByStyleAndVersion(string styleName, string version, CancellationToken cancellationToken)
    {
        var query = new GetExampleLinksByStyleAndVersion.Query(styleName, version);

        var styles = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return styles;
    }

    // GET api/examplelinks/{link}/exists
    [HttpGet("{link}/exists")]
    public async Task<Results<Ok<bool>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> CheckLinkExists(string link, CancellationToken cancellationToken)
    {
        var query = new CheckExampleLinkExistsById.Query(link);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToResultsAsync();

        return exist;
    }

    // GET api/examplelinks/style/{styleName}/exists
    [HttpGet("style/{styleName}/exists")]
    public async Task<Results<Ok<bool>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> CheckLinkWithStyleExists(string styleName, CancellationToken cancellationToken)
    {
        var query = new CheckExampleLinkExistsByStyle.Query(styleName);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToResultsAsync();

        return exist;
    }

    // GET api/examplelinks/noempty
    [HttpGet("no-empty")]
    public async Task<Results<Ok<bool>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> CheckLinksEmpty(CancellationToken cancellationToken)
    {
        var exist = await Sender
            .Send(CheckAnyExampleLinksExist.Query.Singletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { isEmpty = payload })))
            .ToResultsAsync();

        return exist;
    }

    // POST api/examplelinks
    [HttpPost]
    public async Task<Results<Ok<string>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> AddExampleLink([FromBody] AddExampleLinkRequest request, CancellationToken cancellationToken)
    {
        var command = new AddExampleLink.Command
        (
            request.Link,
            request.Style,
            request.Version
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload =>
            {
                if (!string.IsNullOrEmpty(payload))
                {
                    return CreatedAtAction(nameof(CheckLinkExists), new { link = payload }, new { linkId = payload });
                }

                return NoContent();
            }))
            .ToResultsAsync();

        return result;
    }

    // DELETE api/examplelinks/{link}
    [HttpDelete("{link}")]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> DeleteExampleLink(string link, CancellationToken cancellationToken)
    {
        var command = new DeleteExampleLink.Command(link);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(p => p.PrepareErrorResponse())
            .Else(p => p.PrepareOKResponse())
            .ToResultsAsync();

        return result;
    }

    // DELETE api/examplelinks/style/{styleName}
    [HttpDelete("style/{styleName}")]
    public async Task<Results<Ok<BulkDeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> DeleteAllByStyle(string styleName, CancellationToken cancellationToken)
    {
        var command = new DeleteAllExampleLinksByStyle.Command(styleName);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(p => p.PrepareErrorResponse())
            .Else(p => p.PrepareOKResponse())
            .ToResultsAsync();

        return result;
    }
}

// Request DTOs
public sealed record AddExampleLinkRequest(
    string Link,
    string Style,
    string Version
);
