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
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync();

        return properties;
    }

    // GET api/properties//
    [HttpGet]
    public async Task<Results<Ok<List<PropertyQueryResponse>>, BadRequest<ProblemDetails>>> GetAll
        (CancellationToken cancellationToken)
    {
        var properties = await Sender
            .Send(GetAllProperties.Query.Singletone, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsSimpleOkAsync();

        return properties;
    }

    // GET api/properties/{version}/{propertyName}/exists
    [HttpGet("{version}/{propertyName}/exists")]
    public async Task<Results<Ok<bool>, BadRequest<ProblemDetails>>> CheckPropertyExists
    (
        string version,
        string propertyName,
        CancellationToken cancellationToken
    )
    {
        var query = new CheckPropertyExists.Query(version, propertyName);

        var exist = await Sender
            .Send(query, cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse(payload => Ok(new { exists = payload }))
            .ToResultsSimpleOkAsync();

        return exist;
    }

    // POST api/properties/
    [HttpPost]
    public async Task<Results<Created<PropertyCommandResponse>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> AddProperty
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
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareCreateResponse(payload =>
                CreatedAtAction
                (
                    nameof(CheckPropertyExists),
                    new { version = payload.Version, propertyName = payload.PropertyName },
                    payload
                )
            )
            .ToResultsCreatedAsync();

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
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync();

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
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync();

        return result;
    }

    // DELETE api/properties/version/{version}/{propertyName}
    [HttpDelete("version/{version}/{propertyName}")]
    public async Task<Results<Ok<DeleteResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> DeleteProperty(string version, string propertyName, CancellationToken cancellationToken)
    {
        var result = await Sender
            .Send(new DeleteProperty.Command(version, propertyName), cancellationToken)
            .IfErrorsPrepareErrorResponse()
            .ElsePrepareOKResponse()
            .ToResultsOkAsync();

        return result;
    }
}

// Request DTOs
public sealed record PropertyRequest
(
    string Version,
    string PropertyName,
    List<string> Parameters,
    string? DefaultValue = null,
    string? MinValue = null,
    string? MaxValue = null,
    string? Description = null
);

public sealed record PatchPropertyRequest
(
    string Version,
    string PropertyName,
    string CharacteristicToUpdate,
    string? NewValue
);
