using Application.UseCases.Common.Responses;
using Application.UseCases.Properties.Commands;
using Application.UseCases.Properties.Queries;
using Application.UseCases.Properties.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstraction;
using Presentation.Controllers.Utilities;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PropertiesController(ISender sender) : ApiController(sender)
{
    // Queries //

    // GET api/properties/{version}
    [HttpGet("{version}")]
    public async Task<Results<Ok<List<PropertyQueryResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetAllPropertiesByVersion
    (
        string version,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPropertiesByVersion.Query(version);

        var properties = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return properties;
    }

    // GET api/properties//
    [HttpGet]
    public async Task<Results<Ok<List<PropertyQueryResponse>>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetAll
        (CancellationToken cancellationToken)
    {
        var properties = await Sender
            .Send(GetAllProperties.Query.Singletone, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return properties;
    }

    // GET api/properties/{propertyName}/exists
    [HttpGet("{version}/{propertyName}/exists")]
    public async Task<Results<Ok<bool>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> CheckPropertyExists
    (
        string version,
        string propertyName,
        CancellationToken cancellationToken
    )
    {
        var query = new CheckPropertyExists.Query(version, propertyName);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload => Ok(new { exists = payload })))
            .ToResultsAsync();

        return exist;
    }

    // POST api/properties/
    [HttpPost]
    public async Task<Results<Ok<PropertyCommandResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> AddProperty
    (
        [FromBody] PropertyRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AddProperty.Command
        (
            request.Version,
            request.PropertyName,
            request.Parameters,
            request.DefaultValue,
            request.MinValue,
            request.MaxValue,
            request.Description
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse(payload =>
            {
                if (payload is not null)
                {
                    return CreatedAtAction(
                        nameof(CheckPropertyExists),
                        new { version = payload.Version, propertyName = payload.PropertyName },
                        payload
                    );
                }

                return NoContent();
            }))
            .ToResultsAsync();

        return result;
    }

    // PUT api/properties/
    [HttpPut]
    public async Task<Results<Ok<PropertyCommandResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateProperty
    (
        [FromBody] PropertyRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateProperty.Command
        (
            request.Version,
            request.PropertyName,
            request.Parameters,
            request.DefaultValue,
            request.MinValue,
            request.MaxValue,
            request.Description
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return result;
    }

    // PATCH api/properties
    [HttpPatch]
    public async Task<Results<Ok<PropertyCommandResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> PatchProperty
    (
        [FromBody] PatchPropertyRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new PatchProperty.Command
        (
            request.PropertyName,
            request.Version,
            request.CharacteristicToUpdate,
            request.NewValue
        );

        var result = await Sender
            .Send(command, cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return result;
    }

    // DELETE api/properties/version/{version}/{propertyName}
    [HttpDelete("version/{version}/{propertyName}")]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> DeleteProperty(string version, string propertyName, CancellationToken cancellationToken)
    {
        var result = await Sender
            .Send(new DeleteProperty.Command(version, propertyName), cancellationToken)
            .IfErrors(pipeline => pipeline.PrepareErrorResponse())
            .Else(pipeline => pipeline.PrepareOKResponse())
            .ToResultsAsync();

        return result;
    }
}

// Request DTOs
public sealed record PropertyRequest
(
    string PropertyName,
    string Version,
    List<string> Parameters,
    string? DefaultValue = null,
    string? MinValue = null,
    string? MaxValue = null,
    string? Description = null
);

public sealed record PatchPropertyRequest
(
    string PropertyName,
    string Version,
    string CharacteristicToUpdate,
    string? NewValue
);
