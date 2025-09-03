using Application.Features.Properties.Commands;
using Application.Features.Properties.Queries;
using Application.Features.Properties.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PropertiesController(ISender sender) : ApiController(sender)
{
    // GET api/properties/version/{version}
    [HttpGet("version/{version}")]
    [ProducesResponseType<List<PropertyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllByVersion(string version, CancellationToken cancellationToken)
    {
        var query = new GetAllParametersByVersion.Query(version);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // GET api/properties/version/{version}/{propertyName}/exists
    [HttpGet("version/{version}/{propertyName}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckPropertyExists(string version, string propertyName, CancellationToken cancellationToken)
    {
        var query = new CheckPropertyExistsInVersion.Query(version, propertyName);

        var result = await Sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(new { exists = result.Value });
    }

    // POST api/properties/version/{version}
    [HttpPost("version/{version}")]
    [ProducesResponseType<PropertyResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddProperty(string version, [FromBody] AddPropertyRequest request, CancellationToken cancellationToken)
    {
        var command = new AddPropertyInVersion.Command(
            version,
            request.PropertyName,
            request.Parameters,
            request.DefaultValue,
            request.MinValue,
            request.MaxValue,
            request.Description
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(CheckPropertyExists), 
            new { version = result.Value.Version, propertyName = result.Value.PropertyName }, 
            result.Value
        );
    }

    // PUT api/properties/version/{version}/{propertyName}
    [HttpPut("version/{version}/{propertyName}")]
    [ProducesResponseType<PropertyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProperty(string version, string propertyName, [FromBody] UpdatePropertyRequest request, CancellationToken cancellationToken)
    {
        if (version != request.Version || propertyName != request.PropertyName)
            return BadRequest("Route parameters must match payload values");

        var command = new UpdatePropertyForVersion.Command(
            request.Version,
            request.PropertyName,
            request.Parameters,
            request.DefaultValue,
            request.MinValue,
            request.MaxValue,
            request.Description
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // PATCH api/properties/version/{version}/{propertyName}
    [HttpPatch("version/{version}/{propertyName}")]
    [ProducesResponseType<PropertyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchProperty(string version, string propertyName, [FromBody] PatchPropertyRequest request, CancellationToken cancellationToken)
    {
        var command = new PatchPropertyForVersion.Command(
            version,
            propertyName,
            request.CharacteristicToUpdate,
            request.NewValue
        );

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    // DELETE api/properties/version/{version}/{propertyName}
    [HttpDelete("version/{version}/{propertyName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProperty(string version, string propertyName, CancellationToken cancellationToken)
    {
        var command = new DeletePropertyInVersion.Command(version, propertyName);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return NoContent();
    }
}

// Request DTOs
public sealed record AddPropertyRequest(
    string PropertyName,
    List<string> Parameters,
    string? DefaultValue = null,
    string? MinValue = null,
    string? MaxValue = null,
    string? Description = null
);

public sealed record UpdatePropertyRequest(
    string Version,
    string PropertyName,
    List<string> Parameters,
    string? DefaultValue = null,
    string? MinValue = null,
    string? MaxValue = null,
    string? Description = null
);

public sealed record PatchPropertyRequest(
    string CharacteristicToUpdate,
    string? NewValue
);
