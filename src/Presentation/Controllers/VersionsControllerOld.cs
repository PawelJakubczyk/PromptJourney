//using Application.Features.VersionsMaster.Commands;
//using Application.Features.VersionsMaster.Queries;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Presentation.Requests;
//using Microsoft.AspNetCore.Http;

//namespace Presentation.Controllers;

//[ApiController]
//[Route("api/versions")]
//public sealed class VersionsControllerOld : ControllerBase
//{
//    private readonly ISender _sender;

//    public VersionsControllerOld(ISender sender)
//    {
//        _sender = sender;
//    }

//    [HttpPost("{version}/parameters")]
//    [ProducesResponseType(StatusCodes.Status201Created)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> AddParameterToVersion
//    (
//        [FromRoute] string version,
//        [FromBody] AddParameterToVersionRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        if (request is null)
//        {
//            return BadRequest(new { Error = "Request body cannot be null" });
//        }

//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        var command = new AddParameterToVersion.Command
//        {
//            Version = version,
//            PropertyName = request.PropertyName,
//            Parameters = request.Parameters,
//            DefaultValue = request.DefaultValue,
//            MinValue = request.MinValue,
//            MaxValue = request.MaxValue,
//            Description = request.Description
//        };

//        var result = await _sender.Send(command, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return CreatedAtAction(
//                nameof(CheckParameterExists),
//                new { version, propertyName = request.PropertyName },
//                result.Value
//            );
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpPut("{version}/parameters/{propertyName}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> UpdateParameterFromVersion
//    (
//        [FromRoute] string version,
//        [FromRoute] string propertyName,
//        [FromBody] UpdateParameterFromVersionRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        if (request is null)
//        {
//            return BadRequest(new { Error = "Request body cannot be null" });
//        }

//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        if (string.IsNullOrWhiteSpace(propertyName))
//        {
//            return BadRequest(new { Error = "PropertyName parameter cannot be null or empty" });
//        }

//        var command = new UpdateParameterFromVersion.Command
//        {
//            Version = version,
//            PropertyName = propertyName,
//            Parameters = request.Parameters,
//            DefaultValue = request.DefaultValue,
//            MinValue = request.MinValue,
//            MaxValue = request.MaxValue,
//            Description = request.Description
//        };

//        var result = await _sender.Send(command, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(result.Value);
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpPatch("{version}/parameters/{propertyName}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> PatchParameterInVersion
//    (
//        [FromRoute] string version,
//        [FromRoute] string propertyName,
//        [FromBody] PatchParameterInVersionRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        if (request is null)
//        {
//            return BadRequest(new { Error = "Request body cannot be null" });
//        }

//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        if (string.IsNullOrWhiteSpace(propertyName))
//        {
//            return BadRequest(new { Error = "PropertyName parameter cannot be null or empty" });
//        }

//        var command = new PatchParameterInVersionCommand
//        {
//            Version = version,
//            PropertyName = propertyName,
//            PropertyToUpdate = request.PropertyToUpdate,
//            NewValue = request.NewValue
//        };

//        var result = await _sender.Send(command, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(result.Value);
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpDelete("{version}/parameters/{propertyName}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> DeleteParameterInVersion
//    (
//        [FromRoute] string version,
//        [FromRoute] string propertyName,
//        CancellationToken cancellationToken
//    )
//    {
//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        if (string.IsNullOrWhiteSpace(propertyName))
//        {
//            return BadRequest(new { Error = "PropertyName parameter cannot be null or empty" });
//        }

//        var command = new DeleteParameterInVersionCommand
//        {
//            Version = version,
//            PropertyName = propertyName
//        };

//        var result = await _sender.Send(command, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(result.Value);
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpGet("{version}/parameters/{propertyName}/exists")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> CheckParameterExists
//    (
//        [FromRoute] string version,
//        [FromRoute] string propertyName,
//        CancellationToken cancellationToken
//    )
//    {
//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        if (string.IsNullOrWhiteSpace(propertyName))
//        {
//            return BadRequest(new { Error = "PropertyName parameter cannot be null or empty" });
//        }

//        var query = new CheckParameterExistsInVersionQuery
//        {
//            Version = version,
//            PropertyName = propertyName
//        };

//        var result = await _sender.Send(query, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(new { Exists = result.Value });
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpGet("{version}/exists")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> CheckVersionExists
//    (
//        [FromRoute] string version,
//        CancellationToken cancellationToken
//    )
//    {
//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        var query = new CheckVersionExistsInVersionMasterQuery
//        {
//            Version = version
//        };

//        var result = await _sender.Send(query, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(new { Exists = result.Value });
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpGet("{version}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> GetMasterVersionByVersion
//    (
//        [FromRoute] string version,
//        CancellationToken cancellationToken
//    )
//    {
//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        var query = new GetMasterVersionByVersionQuery
//        {
//            Version = version
//        };

//        var result = await _sender.Send(query, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(result.Value);
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }

//    [HttpGet("{version}/parameters")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> GetAllParametersByVersionMaster
//    (
//        [FromRoute] string version,
//        CancellationToken cancellationToken
//    )
//    {
//        if (string.IsNullOrWhiteSpace(version))
//        {
//            return BadRequest(new { Error = "Version parameter cannot be null or empty" });
//        }

//        var query = new GetAllParametersByVersionMasterQuery
//        {
//            Version = version
//        };

//        var result = await _sender.Send(query, cancellationToken);

//        if (result.IsSuccess)
//        {
//            return Ok(result.Value);
//        }

//        return BadRequest(new
//        {
//            Errors = result.Errors.Select(e => e.Message).ToList()
//        });
//    }
//}