using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilities.Errors;
using static Presentation.Constants.StatusPriority;

namespace Presentation.Controllers.Pipeline;

public sealed class Pipeline<TResponse>
{
    internal Result<TResponse> Result { get; }
    internal IActionResult? Response { get; private set; }

    internal Pipeline(Result<TResponse>? result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    internal void SetResponse(IActionResult response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    // Prepare responses
    private Pipeline<TResponse> PrepareResponse
    (
        Func<TResponse?, IActionResult> defaultFactory,
        Func<TResponse?, IActionResult>? customFactory
    )
    {
        var payload = Result.IsSuccess ? Result.Value : default;
        var Response = customFactory != null
            ? customFactory(payload)
            : defaultFactory(payload);

        SetResponse(Response);

        return this;
    }

    public Pipeline<TResponse> PrepareOKResponse(Func<TResponse?, IActionResult>? factory = null)
        => PrepareResponse(response => new OkObjectResult(response), factory);

    public Pipeline<TResponse> PrepareCreateResponse(Func<TResponse?, IActionResult>? factory = null)
        => PrepareResponse(response => new CreatedResult(string.Empty, response), factory);

    public static Error PickHighestPriorityErrorInternal(List<Error> errors)
    {
        Throw.IfNullOrEmpty(errors);

        var mainErrorCode = errors
            .Select(er => er.GetErrorCode())
            .Where(code => code.HasValue)
            .OrderBy(code => StatusPriorityDict.GetValueOrDefault(code!.Value, int.MaxValue))
            .FirstOrDefault() ?? StatusCodes.Status500InternalServerError;

        var mainError = errors
            .Where(e => e.GetErrorCode() == mainErrorCode)
            .FirstOrDefault() ?? errors.First();

        return mainError;
    }
}
