using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Extensions;

namespace Unit.Presentation.Tests.MoqControlersTests;

public abstract class ControllerTestsBase
{
    // Test Helper Methods
    protected static void AssertOkResult<T>(IActionResult actionResult, int expectedCount = -1)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        if (expectedCount >= 0)
        {
            var collection = okResult.Value as IEnumerable<T>;
            collection.Should().HaveCount(expectedCount);
        }
    }

    protected static void AssertCreatedResult<T>(IActionResult actionResult, string expectedActionName)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = actionResult as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(expectedActionName);
        createdResult.Value.Should().NotBeNull();
        createdResult.Value.Should().BeOfType<T>();
    }

    protected static void AssertNoContentResult(IActionResult actionResult)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<NoContentResult>();
    }

    protected static void AssertErrorResult(IActionResult actionResult, int expectedStatusCode)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<ObjectResult>();

        var objResult = actionResult as ObjectResult;
        objResult!.StatusCode.Should().Be(expectedStatusCode);
        objResult.Value.Should().NotBeNull();
    }

    protected static void AssertBadRequestResult(IActionResult actionResult, string? expectedMessage = null)
    {
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<BadRequestObjectResult>();

        if (!string.IsNullOrEmpty(expectedMessage))
        {
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
            badRequestResult.Value.ToString().Should().Contain(expectedMessage);
        }
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
}
