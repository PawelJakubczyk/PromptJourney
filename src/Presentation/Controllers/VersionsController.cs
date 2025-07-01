using Application.Features.Versions.Commands.AddParameterToVersion;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Requests;

namespace Presentation.Controllers;

public sealed class VersionsController : ControllerBase
{
    private readonly ISender _sender;

    public VersionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("api/versions/add-parameter/{version}")]
    public async Task<IActionResult> AddParameterToVersion
    (
        [FromRoute] string version,
        [FromBody] AddParameterToVersionRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request is null)
        {
            return BadRequest("Invalid command");
        }

        var command = new AddParameterToVersionCommand
        {
            Version = version,
            PropertyName = request.PropertyName,
            Parameters = request.Parameters,
            DefaultValue = request.DefaultValue,
            MinValue = request.MinValue,
            MaxValue = request.MaxValue,
            Description = request.Description
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new
        {
            Errors = result.Errors.Select(e => e.Message).ToList()
        });
    }

    //[HttpGet("api/versions/{version}/parameters")]
    //public async Task<IActionResult> GetVersionDetails
    //(
    //    [FromRoute] string version,
    //    CancellationToken cancellationToken
    //)
    //{
    //    var query = new GetVersionDetailsQuery { Version = version };
    //    var result = await _sender.Send(query, cancellationToken);

    //    if (result.IsSuccess)
    //    {
    //        return Ok(result.Value);
    //    }

    //    return BadRequest(new
    //    {
    //        Errors = result.Errors.Select(e => e.Message).ToList()
    //    });
    //}
}