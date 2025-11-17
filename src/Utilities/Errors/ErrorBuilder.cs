using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;

namespace Utilities.Errors;

public class ErrorBuilder {
    private static string _message = ErrorsMessages.ErrorBuilderBaseMessage;
    private int? _errorCode;
    private string? _layer;

    private ErrorBuilder() { }

    public static ErrorBuilder New() => new();

    public ErrorBuilder WithLayer<TLayer>() where TLayer : ILayer {
        _layer = typeof(TLayer).Name;
        return this;
    }

    public ErrorBuilder WithErrorCode(int? errorCode) {
        _errorCode = errorCode;
        return this;
    }

    public ErrorBuilder WithMessage(string message) {
        _message = message;
        return this;
    }

    public Error Build() {
        var error = new Error(_message);

        if (_layer is not null)
            error.Metadata["Layer"] = _layer;

        if (_errorCode is not null)
            error.Metadata["ErrorCode"] = _errorCode;

        return error;
    }
}

public static class ErrorExtensions
{
    public static int? GetErrorCode(this Error error)
    {
        if (error.Metadata.TryGetValue("ErrorCode", out var code) && code is int intCode)
            return intCode;

        return null;
    }

    public static string? GetLayer(this Error error)
    {
        if (error.Metadata.TryGetValue("Layer", out var layer) && layer is string ilayer)
            return ilayer;

        return null;
    }

    public static Dictionary<string, string> GetDetail(this Error error)
    {
        return new Dictionary<string, string> 
        {
            ["ErrorCode"] = (error.GetErrorCode() ?? StatusCodes.Status500InternalServerError).ToString(),
            ["Message"] = error.Message,
            ["Layer"] = error.GetLayer() ?? typeof(UnknownLayer).Name
        };
    }
}
