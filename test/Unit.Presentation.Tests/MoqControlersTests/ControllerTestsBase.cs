using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Extensions;

namespace Unit.Presentation.Tests.MoqControlersTests;

public abstract class ControllerTestsBase
{
    // Test Helper Methods (support both IActionResult and Results<> typed variants)

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
            // value can be null (allowed)
            var value = typedOk.Value;
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

    protected static void AssertCreatedResult<T>(object actionResult, string expectedActionName)
    {
        actionResult.Should().NotBeNull();

        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<CreatedAtActionResult>();

            var createdResult = mvcResult as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(expectedActionName);
            createdResult.Value.Should().NotBeNull();
            createdResult.Value.Should().BeOfType<T>();
            return;
        }

        // Typed Created<T> (Minimal API) - no ActionName available, just validate payload
        if (actionResult is Created<T> typedCreated)
        {
            typedCreated.Value?.Should().BeOfType<T>();
            return;
        }

        throw new AssertionException($"Result is not a Created result. Actual type: {actionResult.GetType().FullName}");
    }

    protected static void AssertNoContentResult(object actionResult)
    {
        actionResult.Should().NotBeNull();

        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<NoContentResult>();
            return;
        }

        // Minimal API NoContent
        if (actionResult is NoContent)
        {
            return;
        }

        throw new AssertionException($"Result is not NoContent. Actual type: {actionResult.GetType().FullName}");
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

        // Generic ObjectResult fallback
        if (actionResult is ObjectResult obj)
        {
            obj.StatusCode.Should().Be(expectedStatusCode);
            obj.Value.Should().NotBeNull();
            return;
        }

        throw new AssertionException($"Result is not an error result. Actual type: {actionResult.GetType().FullName}");
    }

    protected static void AssertBadRequestResult(object actionResult, string? expectedMessage = null)
    {
        actionResult.Should().NotBeNull();

        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<BadRequestObjectResult>();

            if (!string.IsNullOrEmpty(expectedMessage))
            {
                var badRequestResult = mvcResult as BadRequestObjectResult;
                badRequestResult!.Value.Should().NotBeNull();
                badRequestResult.Value.ToString().Should().Contain(expectedMessage);
            }

            return;
        }

        if (actionResult is BadRequest<ProblemDetails> br)
        {
            br.Value.Should().NotBeNull();
            if (!string.IsNullOrEmpty(expectedMessage))
            {
                (br.Value!.Detail ?? br.Value.Title ?? string.Empty).Should().Contain(expectedMessage);
            }

            return;
        }

        if (actionResult is ObjectResult obj)
        {
            obj.Should().BeOfType<BadRequestObjectResult>();
            if (!string.IsNullOrEmpty(expectedMessage))
            {
                obj.Value.Should().NotBeNull();
                obj.Value.ToString().Should().Contain(expectedMessage);
            }

            return;
        }

        throw new AssertionException($"Result is not a BadRequest result. Actual type: {actionResult.GetType().FullName}");
    }

    // Error Response Models for testing
    public class ErrorResponseModel
    {
        public MainErrorModel MainError { get; set; } = new();
        public List<DetailModel> Details { get; set; } = [];
    }

    public class MainErrorModel
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DetailModel
    {
        public string Message { get; set; } = string.Empty;
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