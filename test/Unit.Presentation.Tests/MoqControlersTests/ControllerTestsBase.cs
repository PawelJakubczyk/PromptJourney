using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Utilities.Constants;
using Utilities.Extensions;
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

    protected static void AssertOkResult<T>(object actionResult, int expectedCount = -1)
    {
        actionResult.Should().NotBeNull();

        // Old-style MVC IActionResult
        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<OkObjectResult>();
            var okResult = mvcResult as OkObjectResult;
            okResult!.Value.Should().NotBeNull();

            if (expectedCount >= 0)
            {
                var collection = okResult.Value as IEnumerable<T>;
                collection.Should().HaveCount(expectedCount);
            }

            return;
        }

        // Minimal API typed Ok<T>
        if (actionResult is Ok<T> typedOk)
        {
            var value = typedOk.Value; // allowed to be null
            value?.Should().BeOfType<T>();
            if (expectedCount >= 0)
            {
                if (value is IEnumerable<object> coll)
                    coll.Should().HaveCount(expectedCount);
                else if (value is IEnumerable<T> collT)
                    collT.Should().HaveCount(expectedCount);
                else
                    throw new AssertionException("Expected a collection to assert count against.");
            }
            return;
        }

        // Minimal API union Results<...>
        if (actionResult is IResult unionResult)
        {
            var (status, body) = ExecuteResult(unionResult);
            status.Should().Be(StatusCodes.Status200OK);

            if (expectedCount >= 0)
            {
                // Try to deserialize as collection of T
                var list = JsonSerializer.Deserialize<IEnumerable<T>>(body, _jsonOptions);
                list.Should().NotBeNull();
                list!.Count().Should().Be(expectedCount);
            }
            return;
        }

        // Fallback for OkObjectResult-like
        if (actionResult is OkObjectResult okObj)
        {
            okObj.Value.Should().NotBeNull();
            if (expectedCount >= 0)
            {
                var collection = okObj.Value as IEnumerable<T>;
                collection.Should().HaveCount(expectedCount);
            }

            return;
        }

        throw new AssertionException($"Result is not an OK result. Actual type: {actionResult.GetType().FullName}");
    }



    protected static void AssertErrorResult(object actionResult, int expectedStatusCode)
    {
        actionResult.Should().NotBeNull();

        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<ObjectResult>();

            var objResult = mvcResult as ObjectResult;
            objResult!.StatusCode.Should().Be(expectedStatusCode);
            objResult.Value.Should().NotBeNull();
            return;
        }

        // Typed NotFound<ProblemDetails>
        if (actionResult is NotFound<ProblemDetails> nf)
        {
            nf.Value.Should().NotBeNull();
            nf.Value!.Status.Should().Be(expectedStatusCode);
            return;
        }

        // Typed BadRequest<ProblemDetails>
        if (actionResult is BadRequest<ProblemDetails> br)
        {
            br.Value.Should().NotBeNull();
            br.Value!.Status.Should().Be(expectedStatusCode);
            return;
        }

        // Minimal API union Results<...>
        if (actionResult is IResult unionResult)
        {
            var (status, _) = ExecuteResult(unionResult);
            status.Should().Be(expectedStatusCode);
            return;
        }

        // Generic ObjectResult fallback
        if (actionResult is ObjectResult obj)
        {
            obj.StatusCode.Should().Be(expectedStatusCode);
            obj.Value.Should().NotBeNull();
            return;
        }

        throw new AssertionException($"Result is not an error result. Actual type: {actionResult.GetType().FullName}");
    }

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