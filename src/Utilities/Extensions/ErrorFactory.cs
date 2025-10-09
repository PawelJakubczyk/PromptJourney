using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Utilities.Extensions;

public static class ErrorFactory
{
    public static Error Create()
    {
        return new Error("An error occurred");
    }

    public static Error Withlayer(this Error error, Type layer)
    {
        error.Metadata.TryAdd("Layer", layer.Name.ToString());
        return error;
    }

    public static Error WithErrorCode(this Error error, int? errorCode)
    {
        error.Metadata.TryAdd("ErrorCode", errorCode);
        return error;
    }

    public static Error WithMessage(this Error error, string message)
    {
        var newError = new Error(message);
        foreach (var kvp in error.Metadata)
        {
            newError.Metadata[kvp.Key] = kvp.Value;
        }
        return newError;
    }

    public static int? GetErrorCode(this Error error)
    {
        if (error.Metadata.TryGetValue("ErrorCode", out var code) && code is int intCode)
        {
            return intCode;
        }
        return null;
    }

    public static string? GetLayer(this Error error)
    {
        if (error.Metadata.TryGetValue("Layer", out var layer) && layer is string ilayer)
        {
            return ilayer;
        }
        return null;
    }

    public static Dictionary<string, string> GetDetail(this Error error)
    {
        return new Dictionary<string, string>
        {
            ["ErrorCode"] = (error.GetErrorCode() ?? StatusCodes.Status500InternalServerError).ToString(),
            ["Message"] = error.Message,
            ["Layer"] = error.GetLayer() ?? "UnknownLayer"
        };
    }
}