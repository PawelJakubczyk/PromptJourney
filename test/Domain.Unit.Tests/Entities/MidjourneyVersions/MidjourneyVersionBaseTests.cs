using Domain.Entities.MidjourneyVersions;
using FluentAssertions;

namespace Domain.Unit.Tests.Entities.MidjourneyVersions;

public sealed class MidjourneyVersionBaseTests
{
    [Fact]
    public void MidjourneyVersionBase_Create_ShouldFail_WhenPropertyNameIsEmpty()
    {
        // Arrange
        var version = "5.1";

        // Act
        var result = MidjourneyVersionsBase.Create
        (
            string.Empty, // Empty property name
            version
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be("propertyName cannot be null or empty");
    }
}