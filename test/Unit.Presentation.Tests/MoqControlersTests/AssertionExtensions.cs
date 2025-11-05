using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Unit.Presentation.Tests.MoqControlersTests;

public static class ActionResultAssertionExtensions
{
    public static ErrorResultAssertions BeErrorResult(this ObjectAssertions assertions)
    {
        return new ErrorResultAssertions(assertions.Subject);
    }

    public static CreatedResultAssertions BeCreatedResult(this ObjectAssertions assertions)
    {
        return new CreatedResultAssertions(assertions.Subject);
    }

    public static NoContentResultAssertions BeNoContentResult(this ObjectAssertions assertions)
    {
        return new NoContentResultAssertions(assertions.Subject);
    }

    public static OkResultAssertions BeOkResult(this ObjectAssertions assertions)
    {
        return new OkResultAssertions(assertions.Subject);
    }
}

public sealed class ErrorResultAssertions
{
    private readonly object subject;
    internal ErrorResultAssertions(object subject) => this.subject = subject;

    public ErrorResultAssertions WithStatusCode(int expectedStatus)
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<ObjectResult>();
            var obj = mvcResult as ObjectResult;
            obj!.StatusCode.Should().Be(expectedStatus);
            obj.Value.Should().NotBeNull();
            return this;
        }

        if (subject is NotFound<ProblemDetails> nf)
        {
            nf.Value.Should().NotBeNull();
            nf.Value!.Status.Should().Be(expectedStatus);
            return this;
        }

        if (subject is BadRequest<ProblemDetails> br)
        {
            br.Value.Should().NotBeNull();
            br.Value!.Status.Should().Be(expectedStatus);
            return this;
        }

        if (subject is IResult union)
        {
            var (status, _) = ControllerTestsBase.ExecuteResult(union);
            status.Should().Be(expectedStatus);
            return this;
        }

        if (subject is ObjectResult objRes)
        {
            objRes.StatusCode.Should().Be(expectedStatus);
            objRes.Value.Should().NotBeNull();
            return this;
        }

        throw new Exception($"Result is not an error result. Actual type: {subject.GetType().FullName}");
    }

    public ErrorResultAssertions WithErrorMessage(string expected)
    {
        if (subject is IActionResult mvcResult)
        {
            var obj = mvcResult as ObjectResult;
            obj!.Value.Should().NotBeNull();
            obj.Value.ToString().Should().Contain(expected);
            return this;
        }

        if (subject is NotFound<ProblemDetails> nf)
        {
            nf.Value.Should().NotBeNull();
            (nf.Value!.Detail ?? nf.Value.Title ?? string.Empty).Should().Contain(expected);
            return this;
        }

        if (subject is BadRequest<ProblemDetails> br)
        {
            br.Value.Should().NotBeNull();
            (br.Value!.Detail ?? br.Value.Title ?? string.Empty).Should().Contain(expected);
            return this;
        }

        if (subject is IResult union)
        {
            var (_, body) = ControllerTestsBase.ExecuteResult(union);
            body.Should().Contain(expected);
            return this;
        }

        if (subject is ObjectResult objRes)
        {
            objRes.Value.Should().NotBeNull();
            objRes.Value.ToString().Should().Contain(expected);
            return this;
        }

        throw new Exception($"Result is not an error result. Actual type: {subject.GetType().FullName}");
    }
}

public sealed class CreatedResultAssertions
{
    private readonly object subject;
    internal CreatedResultAssertions(object subject) => this.subject = subject;

    public CreatedResultAssertions WithActionName(string expectedActionName)
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<CreatedAtActionResult>();
            var created = mvcResult as CreatedAtActionResult;
            created!.ActionName.Should().Be(expectedActionName);
            created.Value.Should().NotBeNull();
            return this;
        }

        if (subject is Created<object> typedCreated)
        {
            typedCreated.Value.Should().NotBeNull();
            return this;
        }

        if (subject is IResult union)
        {
            var (status, _) = ControllerTestsBase.ExecuteResult(union);
            status.Should().Be(StatusCodes.Status201Created);
            return this;
        }

        throw new Exception($"Result is not a Created result. Actual type: {subject.GetType().FullName}");
    }

    public CreatedResultAssertions WithValueOfType<T>()
    {
        if (subject is IActionResult mvcResult)
        {
            var created = mvcResult as CreatedAtActionResult;
            created!.Value.Should().BeOfType<T>();
            return this;
        }

        if (subject is Created<T> typedCreated)
        {
            typedCreated.Value?.Should().BeOfType<T>();
            return this;
        }

        if (subject is IResult union)
        {
            var (_, body) = ControllerTestsBase.ExecuteResult(union);

            if (string.IsNullOrWhiteSpace(body))
            {
                // Accept empty body when expecting nullable/reference types
                if (default(T) == null) return this;
                throw new Exception($"Result body is empty but expected a created value of type {typeof(T).FullName}.");
            }

            var obj = JsonSerializer.Deserialize<T>(body, ControllerTestsBase._jsonOptions);
            obj.Should().NotBeNull();
            return this;
        }

        throw new Exception($"Result does not contain a created value of type {typeof(T).FullName}.");
    }
}

public sealed class NoContentResultAssertions
{
    private readonly object subject;
    internal NoContentResultAssertions(object subject) => this.subject = subject;

    public NoContentResultAssertions BeNoContent()
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<NoContentResult>();
            return this;
        }

        if (subject is NoContent)
        {
            return this;
        }

        if (subject is IResult union)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(union);
            status.Should().Be(StatusCodes.Status204NoContent);
            body.Should().BeNullOrEmpty();
            return this;
        }

        throw new Exception($"Result is not NoContent. Actual type: {subject.GetType().FullName}");
    }
}

public sealed class OkResultAssertions
{
    private readonly object subject;
    internal OkResultAssertions(object subject) => this.subject = subject;

    public OkResultAssertions WithCount(int expectedCount)
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<OkObjectResult>();
            var ok = mvcResult as OkObjectResult;
            ok!.Value.Should().NotBeNull();
            var collection = ok.Value as IEnumerable<object>;
            collection.Should().HaveCount(expectedCount);
            return this;
        }

        if (subject is IResult union)
        {
            var (_, body) = ControllerTestsBase.ExecuteResult(union);
            var list = JsonSerializer.Deserialize<IEnumerable<object>>(body, ControllerTestsBase._jsonOptions);
            list.Should().NotBeNull();
            list!.Count().Should().Be(expectedCount);
            return this;
        }

        if (subject is Ok<object> typedOk)
        {
            var value = typedOk.Value;
            if (value is IEnumerable<object> coll)
            {
                coll.Should().HaveCount(expectedCount);
                return this;
            }

            throw new Exception("Expected a collection to assert count against.");
        }

        throw new Exception($"Result is not an OK result. Actual type: {subject.GetType().FullName}");
    }

    public OkResultAssertions WithValueOfType<T>()
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<OkObjectResult>();
            var ok = mvcResult as OkObjectResult;
            ok!.Value.Should().BeOfType<T>();
            return this;
        }

        if (subject is Ok<T> typedOk)
        {
            typedOk.Value?.Should().BeOfType<T>();
            return this;
        }

        if (subject is IResult union)
        {
            var (_, body) = ControllerTestsBase.ExecuteResult(union);

            if (string.IsNullOrWhiteSpace(body))
            {
                // Accept empty body when expecting nullable/reference types
                if (default(T) == null) return this;
                throw new Exception($"Result body is empty but expected a value of type {typeof(T).FullName}.");
            }

            var obj = JsonSerializer.Deserialize<T>(body, ControllerTestsBase._jsonOptions);
            obj.Should().NotBeNull();
            return this;
        }

        throw new Exception($"Result is not an OK result with a value of type {typeof(T).FullName}.");
    }
}