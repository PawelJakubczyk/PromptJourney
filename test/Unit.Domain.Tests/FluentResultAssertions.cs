using FluentAssertions;
using FluentResults;

namespace Unit.Domain.Tests;

public static class FluentResultAssertions
{
    public static void ShouldBeSuccess<T>(this Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
        result.Value.Should().NotBeNull();
    }

    public static void ShouldBeFailure<T>(this Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
    }
}
