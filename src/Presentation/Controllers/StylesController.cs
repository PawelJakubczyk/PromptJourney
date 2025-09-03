using Application.Features.Styles.Commands.AddStyle;
using Application.Features.Styles.Commands.AddTagToStyle;
using Application.Features.Styles.Commands.RemoveStyle;
using Application.Features.Styles.Commands.RemoveTagInStyle;
using Application.Features.Styles.Commands;
using Application.Features.Styles.Queries;
using Application.Features.Styles.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StylesController(ISender sender) : ApiController(sender)
{
    // GET api/styles
    [HttpGet]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllStyles.Query();

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // GET api/styles/{name}
    [HttpGet("{name}")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var query = new GetStyleByName.Query(name);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/styles/by-type/{type}
    [HttpGet("by-type/{type}")]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByType(string type, CancellationToken cancellationToken)
    {
        var query = new GetStylesByType.Query(type);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/styles/by-tags?tags=tag1&tags=tag2
    [HttpGet("by-tags")]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByTags([FromQuery] List<string> tags, CancellationToken cancellationToken)
    {
        var query = new GetStylesByTags.Query(tags);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    // GET api/styles/by-description?keyword=forest
    [HttpGet("by-description")]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDescription([FromQuery] string keyword, CancellationToken cancellationToken)
    {
        var query = new GetStylesByDescriptionKeyword.Query(keyword);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(result.Value);
    }

    // GET api/styles/{name}/exists
    [HttpGet("{name}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckExists(string name, CancellationToken cancellationToken)
    {
        var query = new CheckStyleExist.Query(name);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(new { exists = result.Value });
    }

    // GET api/styles/{styleName}/tags/{tag}/exists
    [HttpGet("{styleName}/tags/{tag}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckTagExists(string styleName, string tag, CancellationToken cancellationToken)
    {
        var query = new CheckTagExistInStyle.Query(styleName, tag);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        return Ok(new { exists = result.Value });
    }

    // POST api/styles
    [HttpPost]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStyleRequest request, CancellationToken cancellationToken)
    {
        var command = new AddStyle.Command(
            request.Name,
            request.Type,
            request.Description,
            request.Tags
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(nameof(GetByName), new { name = result.Value.Name }, result.Value);
    }

    // PUT api/styles/{name}
    [HttpPut("{name}")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string name, [FromBody] UpdateStyleRequest request, CancellationToken cancellationToken)
    {
        if (name != request.Name)
            return BadRequest("Route name and payload name must match");

        var command = new UpdateStyle.Command(
            request.Name,
            request.Type,
            request.Description,
            request.Tags
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // DELETE api/styles/{name}
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string name, CancellationToken cancellationToken)
    {
        var command = new DeleteStyle.Command(name);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return NoContent();
    }

    // POST api/styles/{name}/tags
    [HttpPost("{name}/tags")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(string name, [FromBody] AddTagRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTagToStyle.Command(name, request.Tag);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // DELETE api/styles/{name}/tags/{tag}
    [HttpDelete("{name}/tags/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTag(string name, string tag, CancellationToken cancellationToken)
    {
        var command = new DeleteTagInStyle.Command(name, tag);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }

    // PUT api/styles/{name}/description
    [HttpPut("{name}/description")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription(string name, [FromBody] UpdateDescriptionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDescriptionInStyle.Command(name, request.Description);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }
}

// Request DTOs
public sealed record CreateStyleRequest(
    string Name,
    string Type,
    string? Description = null,
    List<string>? Tags = null
);

public sealed record UpdateStyleRequest(
    string Name,
    string Type,
    string? Description = null,
    List<string>? Tags = null
);

public sealed record AddTagRequest(string Tag);

public sealed record UpdateDescriptionRequest(string Description);