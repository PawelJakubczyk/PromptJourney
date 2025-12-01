using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Unit.Presentation.Tests.MoqControlersTests;

public static class ActionResultAssertionExtensions
{
    public static BadRequestResultAssertions BeBadRequestResult(this ObjectAssertions assertions)
    {
        return new BadRequestResultAssertions(assertions.Subject);
    }

    public static NotFoundResultAssertions BeNotFoundResult(this ObjectAssertions assertions)
    {
        return new NotFoundResultAssertions(assertions.Subject);
    }

    public static ConflictResultAssertions BeConflictResult(this ObjectAssertions assertions)
    {
        return new ConflictResultAssertions(assertions.Subject);
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

public sealed class BadRequestResultAssertions
{
    private readonly object subject;
    internal BadRequestResultAssertions(object subject) => this.subject = subject;

    public BadRequestResultAssertions WithMessage(string expected)
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<ObjectResult>();
            var obj = mvcResult as ObjectResult;
            obj!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            obj.Value.Should().NotBeNull();
            obj.Value.ToString()!.Should().Contain(expected);
            return this;
        }

        if (subject is BadRequest<ProblemDetails> br)
        {
            br.Value.Should().NotBeNull();
            br.Value!.Status.Should().Be(StatusCodes.Status400BadRequest);
            (br.Value.Detail ?? br.Value.Title ?? string.Empty).Should().Contain(expected);
            return this;
        }

        if (subject is IResult result)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(result);
            status.Should().Be(StatusCodes.Status400BadRequest);
            body.Should().Contain(expected);
            return this;
        }

        throw new Exception($"Result is not a BadRequest result. Actual type: {subject.GetType().FullName}");
    }
}

public sealed class NotFoundResultAssertions
{
    private readonly object subject;
    internal NotFoundResultAssertions(object subject) => this.subject = subject;

    public NotFoundResultAssertions WithMessage(string expected)
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<ObjectResult>();
            var obj = mvcResult as ObjectResult;
            obj!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            obj.Value.Should().NotBeNull();
            obj.Value.ToString()!.Should().Contain(expected);
            return this;
        }

        if (subject is NotFound<ProblemDetails> nf)
        {
            nf.Value.Should().NotBeNull();
            nf.Value!.Status.Should().Be(StatusCodes.Status404NotFound);
            (nf.Value.Detail ?? nf.Value.Title ?? string.Empty).Should().Contain(expected);
            return this;
        }

        if (subject is IResult result)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(result);
            status.Should().Be(StatusCodes.Status404NotFound);
            body.Should().Contain(expected);
            return this;
        }

        throw new Exception($"Result is not a NotFound result. Actual type: {subject.GetType().FullName}");
    }
}

public sealed class ConflictResultAssertions
{
    private readonly object subject;
    internal ConflictResultAssertions(object subject) => this.subject = subject;

    public ConflictResultAssertions WithMessage(string expected)
    {
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<ObjectResult>();
            var obj = mvcResult as ObjectResult;
            obj!.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            obj.Value.Should().NotBeNull();
            obj.Value.ToString()!.Should().Contain(expected);
            return this;
        }

        if (subject is Conflict<ProblemDetails> cf)
        {
            cf.Value.Should().NotBeNull();
            cf.Value!.Status.Should().Be(StatusCodes.Status409Conflict);
            (cf.Value.Detail ?? cf.Value.Title ?? string.Empty).Should().Contain(expected);
            return this;
        }

        if (subject is IResult result)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(result);
            status.Should().Be(StatusCodes.Status409Conflict);
            body.Should().Contain(expected);
            return this;
        }

        throw new Exception($"Result is not a Conflict result. Actual type: {subject.GetType().FullName}");
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

    public CreatedResultAssertions WithValue<TValue>(TValue expected)
    {
        // Handle MVC IActionResult
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<CreatedAtActionResult>();
            var created = mvcResult as CreatedAtActionResult;
            created!.Value.Should().NotBeNull();
            created.Value.Should().Be(expected);
            return this;
        }

        // Handle Created<TValue> directly
        if (subject is Created<TValue> typedCreated)
        {
            typedCreated.Value.Should().Be(expected);
            return this;
        }

        // Handle Results<> (Minimal API IResult)
        if (subject is IResult result)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(result);
            status.Should().Be(StatusCodes.Status201Created);

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new Exception(
                    $"Result body is empty but expected a created value of type {typeof(TValue).FullName}."
                );
            }

            var deserialized = JsonSerializer.Deserialize<TValue>(body, ControllerTestsBase._jsonOptions);
            deserialized.Should().BeEquivalentTo(expected);
            return this;
        }

        throw new Exception(
            $"Result is not a Created result with a value. Actual type: {subject.GetType().FullName}"
        );
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

    public OkResultAssertions WithValue<TValue>(TValue expected)
    {
        // Handle IActionResult (MVC style)
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<OkObjectResult>();
            var ok = mvcResult as OkObjectResult;
            ok!.Value.Should().NotBeNull();
            ok.Value.Should().Be(expected);
            return this;
        }

        // Handle Ok<TValue> directly
        if (subject is Ok<TValue> typedOk)
        {
            typedOk.Value.Should().Be(expected);
            return this;
        }

        // Handle Results<> union types
        if (subject is IResult result)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(result);
            status.Should().Be(StatusCodes.Status200OK);

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new Exception($"Result body is empty but expected a value of type {typeof(TValue).FullName}.");
            }

            var deserialized = JsonSerializer.Deserialize<TValue>(body, ControllerTestsBase._jsonOptions);
            deserialized.Should().BeEquivalentTo(expected);
            return this;
        }

        throw new Exception($"Result is not an OK result with value. Actual type: {subject.GetType().FullName}");
    }

    public OkResultAssertions WithNullValue()
    {
        // Handle IActionResult (MVC style)
        if (subject is IActionResult mvcResult)
        {
            mvcResult.Should().BeOfType<OkObjectResult>();
            var ok = mvcResult as OkObjectResult;
            ok!.Value.Should().BeNull();
            return this;
        }

        // Handle Ok<T> directly
        var subjectType = subject.GetType();
        if (subjectType.IsGenericType && subjectType.GetGenericTypeDefinition() == typeof(Ok<>))
        {
            var valueProp = subjectType.GetProperty("Value");
            var value = valueProp!.GetValue(subject);
            value.Should().BeNull();
            return this;
        }

        // Handle Results<> union types (IResult)
        if (subject is IResult result)
        {
            var (status, body) = ControllerTestsBase.ExecuteResult(result);
            status.Should().Be(StatusCodes.Status200OK);

            if (!string.IsNullOrWhiteSpace(body))
            {
                throw new Exception($"Expected null value, but response body was not empty: '{body}'.");
            }

            return this;
        }

        throw new Exception($"Result is not an OK result. Actual type: {subject.GetType().FullName}");
    }
}