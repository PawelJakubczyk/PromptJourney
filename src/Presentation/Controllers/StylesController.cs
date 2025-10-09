using Application.Features.Styles.Commands;
using Application.Features.Styles.Commands.AddStyle;
using Application.Features.Styles.Commands.AddTagToStyle;
using Application.Features.Styles.Commands.RemoveStyle;
using Application.Features.Styles.Commands.RemoveTagInStyle;
using Application.Features.Styles.Queries;
using Application.Features.Styles.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.ControllersUtilities;

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
        return await Sender
            .Send(new GetAllStyles.Query(), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/styles/{name}
    [HttpGet("{name}")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetStyleByName.Query(name), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/styles/by-type/{type}
    [HttpGet("by-type/{type}")]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByType(string type, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetStylesByType.Query(type), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/styles/by-tags?tags=tag1&tags=tag2
    [HttpGet("by-tags")]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByTags([FromQuery] List<string> tags, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetStylesByTags.Query(tags), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/styles/by-description?keyword=forest
    [HttpGet("by-description")]
    [ProducesResponseType<List<StyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDescription([FromQuery] string keyword, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new GetStylesByDescriptionKeyword.Query(keyword), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/styles/{name}/exists
    [HttpGet("{name}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckExists(string name, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckStyleExist.Query(name), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToActionResultAsync();
    }

    // GET api/styles/{styleName}/tags/{tag}/exists
    [HttpGet("{styleName}/tags/{tag}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckTagExists(string styleName, string tag, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckTagExistInStyle.Query(styleName, tag), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToActionResultAsync();
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

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload =>
            {
                if (payload is not null)
                {
                    return CreatedAtAction(nameof(GetByName), new { name = ((StyleResponse)payload).Name }, payload);
                }

                return NoContent();
            }))
            .ToActionResultAsync();
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

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // DELETE api/styles/{name}
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string name, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new DeleteStyle.Command(name), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(_ => NoContent()))
            .ToActionResultAsync();
    }

    // POST api/styles/{name}/tags
    [HttpPost("{name}/tags")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(string name, [FromBody] AddTagRequest request, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new AddTagToStyle.Command(name, request.Tag), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // DELETE api/styles/{name}/tags/{tag}
    [HttpDelete("{name}/tags/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTag(string name, string tag, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new DeleteTagFromStyle.Command(name, tag), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(_ => NoContent()))
            .ToActionResultAsync();
    }

    // PUT api/styles/{name}/description
    [HttpPut("{name}/description")]
    [ProducesResponseType<StyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription(string name, [FromBody] UpdateDescriptionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDescriptionInStyle.Command(name, request.Description);

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
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