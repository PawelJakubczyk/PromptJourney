using Application.Features.Properties.Commands;
using Application.Features.Properties.Queries;
using Application.Features.Properties.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.ControllersUtilities;

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
        return await Sender
            .Send(new GetAllParametersByVersion.Query(version), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // GET api/properties/version/{version}/{propertyName}/exists
    [HttpGet("version/{version}/{propertyName}/exists")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckPropertyExists(string version, string propertyName, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new CheckPropertyExistsInVersion.Query(version, propertyName), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToActionResultAsync();
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

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload =>
            {
                if (payload is not null)
                {
                    return CreatedAtAction(
                        nameof(CheckPropertyExists),
                        new { version = ((PropertyResponse)payload).Version, propertyName = ((PropertyResponse)payload).PropertyName },
                        payload
                    );
                }

                return NoContent();
            }))
            .ToActionResultAsync();
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

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
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

        return await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToActionResultAsync();
    }

    // DELETE api/properties/version/{version}/{propertyName}
    [HttpDelete("version/{version}/{propertyName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProperty(string version, string propertyName, CancellationToken cancellationToken)
    {
        return await Sender
            .Send(new DeletePropertyInVersion.Command(version, propertyName), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(_ => NoContent()))
            .ToActionResultAsync();
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
