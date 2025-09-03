using Application.Features.ExampleLinks.Commands;
using Application.Features.ExampleLinks.Queries;
using Application.Features.ExampleLinks.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ExampleLinksController(ISender sender) : ApiController(sender)
{
    // GET api/examplelinks
    [HttpGet]
    [ProducesResponseType<List<ExampleLinkRespose>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllExampleLinks.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(result.Value);
    }

    // GET api/examplelinks/style/{styleName}
    [HttpGet("style/{styleName}")]
    [ProducesResponseType<List<ExampleLinkRespose>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStyle(string styleName, CancellationToken cancellationToken)
    {
        var query = new GetExampleLinksByStyle.Query(styleName);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/examplelinks/style/{styleName}/version/{version}
    [HttpGet("style/{styleName}/version/{version}")]
    [ProducesResponseType<List<ExampleLinkRespose>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStyleAndVersion(string styleName, string version, CancellationToken cancellationToken)
    {
        var query = new GetExampleLinksByStyleAndVersion.Query(styleName, version);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/examplelinks/{link}/exists
    [HttpGet("{link}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckLinkExists(string link, CancellationToken cancellationToken)
    {
        var query = new CheckExampleLinkExist.Query(link);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(new { exists = result.Value });
    }

    // GET api/examplelinks/style/{styleName}/exists
    [HttpGet("style/{styleName}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckLinkWithStyleExists(string styleName, CancellationToken cancellationToken)
    {
        var query = new CheckExampleLinkWithStyleExists.Query(styleName);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(new { exists = result.Value });
    }

    // GET api/examplelinks/empty
    [HttpGet("empty")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckLinksEmpty(CancellationToken cancellationToken)
    {
        var query = new CheckExampleLinksAreNotEmpty.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(new { isEmpty = result.Value });
    }

    // POST api/examplelinks
    [HttpPost]
    [ProducesResponseType<ExampleLinkRespose>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddExampleLink([FromBody] AddExampleLinkRequest request, CancellationToken cancellationToken)
    {
        var command = new AddExampleLink.Command(
            request.Link,
            request.Style,
            request.Version
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(CheckLinkExists), 
            new { link = result.Value.Link }, 
            result.Value
        );
    }

    // DELETE api/examplelinks/{link}
    [HttpDelete("{link}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteExampleLink(string link, CancellationToken cancellationToken)
    {
        var command = new DeleteExampleLink.Command(link);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return NoContent();
    }

    // DELETE api/examplelinks/style/{styleName}
    [HttpDelete("style/{styleName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAllByStyle(string styleName, CancellationToken cancellationToken)
    {
        var command = new DeleteAllExampleLinksByStyle.Command(styleName);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return NoContent();
    }
}

// Request DTOs
public sealed record AddExampleLinkRequest(
    string Link,
    string Style,
    string Version
);