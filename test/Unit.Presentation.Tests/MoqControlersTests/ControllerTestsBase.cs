using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Utilities.Constants;
using Utilities.Errors;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Unit.Presentation.Tests.MoqControlersTests;

public abstract class ControllerTestsBase
{
    // Test Helper Methods (support both IActionResult and Results<> typed variants)

    internal static (int Status, string Body) ExecuteResult(IResult result)
    {
        // Provide minimal services required by typed results during execution
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddRouting();
        services.Configure<HttpJsonOptions>(_ => { });
        // Optional, but safe for ProblemDetails scenarios
        services.AddProblemDetails();

        var provider = services.BuildServiceProvider();

        var ctx = new DefaultHttpContext
        {
            RequestServices = provider
        };

        using var ms = new MemoryStream();
        ctx.Response.Body = ms;

        result.ExecuteAsync(ctx).GetAwaiter().GetResult();

        ms.Position = 0;
        var body = new StreamReader(ms).ReadToEnd();
        return (ctx.Response.StatusCode, body);
    }

    internal static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Helper method to create error results
    protected static Result<TResult> CreateFailureResult<TResult, TLayer>(int statusCode, string message)
        where TLayer : ILayer
    {
        var error = ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(message)
            .WithErrorCode(statusCode)
            .Build();

        return Result.Fail<TResult>(error);
    }

    // Custom assertion exception to keep failures explicit
    private class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}