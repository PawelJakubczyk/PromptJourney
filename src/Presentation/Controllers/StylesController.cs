using Application.UseCases.Common.Responses;
using Application.UseCases.Styles.Commands;
using Application.UseCases.Styles.Queries;
using Application.UseCases.Styles.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.Pipeline;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StylesController(ISender sender) : ApiController(sender)
{
    // Queries //
    // GET api/styles
    [HttpGet]
    public async Task<Results<Ok<List<StyleResponse>>, BadRequest<ProblemDetails>>> GetAll(CancellationToken cancellationToken) 
    {
        var query = GetAllStyles.Query.Singleton;

        var styles = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<List<StyleResponse>, BadRequest<ProblemDetails>>(HttpContext);

        return styles;
    }

    // GET api/styles/{name}
    [HttpGet("{name}")]
    public async Task<Results<Ok<StyleResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByName
    (
        string name, 
        CancellationToken cancellationToken
    ) 
    {
        var query = new GetStyleByName.Query(name);

        var style = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<StyleResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return style;
    }

    // GET api/styles/by-type/{type}
    [HttpGet("by-type/{type}")]
    public async Task<Results<Ok<List<StyleResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByType
    (
        string type, 
        CancellationToken cancellationToken
    ) 
    {
        var query = new GetStylesByType.Query(type);

        var styles = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<List<StyleResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return styles;
    }

    // GET api/styles/by-tags?tags=tag1&tags=tag2
    [HttpGet("by-tags")]
    public async Task<Results<Ok<List<StyleResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetByTags
    (
        [FromQuery] List<string?> tags, 
        CancellationToken cancellationToken
    ) 
    {
        var query = new GetStylesByTags.Query(tags);

        var styles = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<List<StyleResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return styles;
    }

    // GET api/styles/by-description?keyword=forest
    [HttpGet("by-description")]
    public async Task<Results<Ok<List<StyleResponse>>, BadRequest<ProblemDetails>>> GetByDescription
    (
        [FromQuery] string keyword, 
        CancellationToken cancellationToken
    ) 
    {
        var query = new GetStylesByDescriptionKeyword.Query(keyword);

        var styles = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<List<StyleResponse>, BadRequest<ProblemDetails>>(HttpContext);

        return styles;
    }

    // GET api/styles/{name}/exists
    [HttpGet("{name}/exists")]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckExists
    (
        string name, 
        CancellationToken cancellationToken
    ) 
    {
        var query = new CheckStyleExists.Query(name);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exist;
    }

    // GET api/styles/{styleName}/tags/{tag}/exists
    [HttpGet("{styleName}/tags/{tag}/exists")]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckTagExists
    (
        string styleName, 
        string tag, 
        CancellationToken cancellationToken
    ) 
    {
        var query = new CheckTagExistsInStyle.Query(styleName, tag);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(payload))
            .ToResultsOkAsync<bool, BadRequest<ProblemDetails>>(HttpContext);

        return exist;
    }

    // Commands //

    // POST api/styles
    [HttpPost]
    public async Task<Results<Created<StyleResponse>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> Create
    (
        [FromBody] CreateStyleRequest request,
        CancellationToken cancellationToken
    ) 
    {
        var command = new AddStyle.Command
        (
            request.Name,
            request.Type,
            request.Description,
            request.Tags
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareCreateResponse()
            .ToResultsCreatedAsync<StyleResponse, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>
            (
                locationFactory: version => $"/api/styles/{request.Name}",
                httpContext: HttpContext
            );

        return result;
    }

    // PUT api/styles/
    [HttpPut]
    public async Task<Results<Ok<StyleResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> Update
    (
        [FromBody] UpdateStyleRequest request,
        CancellationToken cancellationToken
    ) 
    {
        var command = new UpdateStyle.Command
        (
            request.Name,
            request.Type,
            request.Description,
            request.Tags
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<StyleResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>
            (
                HttpContext
            );

        return result;
    }

    // DELETE api/styles/{name}
    [HttpDelete("{name}")]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> Delete
    (
        string name, 
        CancellationToken cancellationToken
    ) 
    {
        var command = new DeleteStyle.Command(name);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<DeleteResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // POST api/styles/{name}/tags/{tag}
    [HttpPost("{name}/tags/{tag}")]
    public async Task<Results<Ok<string>, NotFound<ProblemDetails>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> AddTag
    (
        string name, 
        string tag, 
        CancellationToken cancellationToken
    ) 
    {
        var command = new AddTagToStyle.Command(name, tag);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(new { tag = payload, message = $"Tag '{payload}' added successfully" }))
            .ToResultsOkAsync<string, NotFound<ProblemDetails>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // DELETE api/styles/{name}/tags/{tag}
    [HttpDelete("{name}/tags/{tag}")]
    public async Task<Results<Ok<StyleResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> RemoveTag
    (
        string name, 
        string tag, 
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteTagFromStyle.Command(name, tag);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync<StyleResponse, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);

        return result;
    }

    // PUT api/styles/{name}/description
    [HttpPut("{name}/description")]
    public async Task<Results<Ok<string?>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateDescription
    (
        string name, 
        [FromBody] UpdateDescriptionRequest request, 
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateDescriptionInStyle.Command(name, request.Description);

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(new { description = payload }))
            .ToResultsOkAsync<string?, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>(HttpContext);
        return result;
    }
}

// Request DTOs
public sealed record CreateStyleRequest(
    string Name,
    string Type,
    string? Description = null,
    List<string?>? Tags = null
);

public sealed record UpdateStyleRequest(
    string Name,
    string Type,
    string? Description = null,
    List<string?>? Tags = null
);

public sealed record AddTagRequest(string Tag);

public sealed record UpdateDescriptionRequest(string? Description);
