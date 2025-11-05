using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Unit.Presentation.Tests.MoqControlersTests;

public static class AssertionExtensions
{
    public static void ShouldBeOkResult<T>(this object actionResult, int expectedCount = -1)
    {
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
                    throw new Exception("Expected a collection to assert count against.");
            }
            return;
        }

        // Minimal API union Results<...>
        if (actionResult is IResult unionResult)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(unionResult);
            status.Should().Be(StatusCodes.Status200OK);

            if (expectedCount >= 0)
            {
                var list = JsonSerializer.Deserialize<IEnumerable<T>>(body, ControllerTestsBase._jsonOptions);
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

        throw new Exception($"Result is not an OK result. Actual type: {actionResult.GetType().FullName}");
    }

    public static void ShouldBeCreatedResult<T>(this object actionResult, string expectedActionName)
    {
        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<CreatedAtActionResult>();

            var createdResult = mvcResult as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(expectedActionName);
            createdResult.Value.Should().NotBeNull();
            createdResult.Value.Should().BeOfType<T>();
            return;
        }

        // Typed Created<T> (Minimal API)
        if (actionResult is Created<T> typedCreated)
        {
            typedCreated.Value?.Should().BeOfType<T>();
            return;
        }

        // Minimal API union Results<...>
        if (actionResult is IResult unionResult)
        {
            var (status, _) = ControllerTestsBase.ExecuteResult(unionResult);
            status.Should().Be(StatusCodes.Status201Created);
            return;
        }

        throw new Exception($"Result is not a Created result. Actual type: {actionResult.GetType().FullName}");
    }

    public static void ShouldBeNoContentResult(this object actionResult)
    {
        if (actionResult is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<NoContentResult>();
            return;
        }

        if (actionResult is NoContent)
        {
            return;
        }

        if (actionResult is IResult unionResult)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(unionResult);
            status.Should().Be(StatusCodes.Status204NoContent);
            body.Should().BeNullOrEmpty();
            return;
        }

        throw new Exception($"Result is not NoContent. Actual type: {actionResult.GetType().FullName}");
    }

    public static void ShouldBeErrorResult(this object actionResult, int expectedStatusCode)
    {
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
            var (status, _) = ControllerTestsBase.ExecuteResult(unionResult);
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

        throw new Exception($"Result is not an error result. Actual type: {actionResult.GetType().FullName}");
    }

    public static void ShouldBeBadRequestResult(this object actionResult, string? expectedMessage = null)
    {
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

        if (actionResult is IResult unionResult)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(unionResult);
            status.Should().Be(StatusCodes.Status400BadRequest);
            if (!string.IsNullOrEmpty(expectedMessage))
            {
                body.Should().Contain(expectedMessage);
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

        throw new Exception($"Result is not a BadRequest result. Actual type: {actionResult.GetType().FullName}");
    }
}
