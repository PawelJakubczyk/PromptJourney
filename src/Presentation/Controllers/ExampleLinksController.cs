using Application.Features.ExampleLinks.Commands;
using Application.Features.ExampleLinks.Queries;
using Application.Features.ExampleLinks.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType<List<ExampleLinkResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetAllExampleLinks.Query(), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/examplelinks/style/{styleName}
    [HttpGet("style/{styleName}")]
    [ProducesResponseType<List<ExampleLinkResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStyle(string styleName, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetExampleLinksByStyle.Query(styleName), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/examplelinks/style/{styleName}/version/{version}
    [HttpGet("style/{styleName}/version/{version}")]
    [ProducesResponseType<List<ExampleLinkResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStyleAndVersion(string styleName, string version, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetExampleLinksByStyleAndVersion.Query(styleName, version), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/examplelinks/{link}/exists
    [HttpGet("{link}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckLinkExists(string link, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckExampleLinkExist.Query(link), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToActionResultAsync();
    }

    // GET api/examplelinks/style/{styleName}/exists
    [HttpGet("style/{styleName}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckLinkWithStyleExists(string styleName, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckExampleLinkWithStyleExists.Query(styleName), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToActionResultAsync();
    }

    // GET api/examplelinks/noempty
    [HttpGet("noempty")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckLinksEmpty(CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckAnyExampleLinksExist.Query(), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { isEmpty = payload })))
            .ToActionResultAsync();
    }

    // POST api/examplelinks
    [HttpPost]
    [ProducesResponseType<ExampleLinkResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddExampleLink([FromBody] AddExampleLinkRequest request, CancellationToken cancellationToken)
    {
        var command = new AddExampleLink.Command(
            request.Link,
            request.Style,
            request.Version
        );

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload =>
            {
                if (payload is not null)
                {
                    return CreatedAtAction(nameof(CheckLinkExists), new { link = ((dynamic)payload).Link }, payload);
                }

                return NoContent();
            }))
            .ToActionResultAsync();
    }

    // DELETE api/examplelinks/{link}
    [HttpDelete("{link}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteExampleLink(string link, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new DeleteExampleLink.Command(link), cancellationToken)
            .IfErrors(p => p.PrepareErrorResponse())
            .Else(p => p.PrepareOKResponse(_ => NoContent()))
            .ToActionResultAsync();
    }

    // DELETE api/examplelinks/style/{styleName}
    [HttpDelete("style/{styleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAllByStyle(string styleName, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new DeleteAllExampleLinksByStyle.Command(styleName), cancellationToken)
            .IfErrors(p => p.PrepareErrorResponse())
            .Else(p => p.PrepareOKResponse(_ => NoContent()))
            .ToActionResultAsync();
    }
}

// Request DTOs
public sealed record AddExampleLinkRequest(
    string Link,
    string Style,
    string Version
);