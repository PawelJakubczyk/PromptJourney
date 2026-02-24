using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Errors;

namespace Unit.Utilities.Tests.Extensions;

public class ErrorFactoryTests
{
    [Fact]
    public void Create_ShouldReturnErrorWithDefaultMessage()
    {
        // Act
        var error = ErrorBuilder.New().Build();

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("An error occurred");
        error.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Withlayer_ShouldAddLayerToMetadata_AndReturnSameBuilder()
    {
        // Arrange
        var builder = ErrorBuilder.New();

        // Act
        var returned = builder.WithLayer<DomainLayer>();
        var error = returned.Build();

        // Assert
        returned.Should().BeSameAs(builder);
        error.Metadata.Should().ContainKey("Layer");
        error.Metadata["Layer"].Should().Be("DomainLayer");
    }

    [Fact]
    public void WithErrorCode_ShouldAddErrorCodeToMetadata()
    {
        // Arrange
        var builder = ErrorBuilder.New();
        var errorCode = StatusCodes.Status400BadRequest;

        // Act
        var returned = builder.WithErrorCode(errorCode);
        var error = returned.Build();

        // Assert
        returned.Should().BeSameAs(builder);
        error.Metadata.Should().ContainKey("ErrorCode");
        error.Metadata["ErrorCode"].Should().Be(errorCode);
    }

    [Fact]
    public void WithErrorCode_WithNullValue_ShouldNotAddErrorCodeToMetadata()
    {
        // Arrange
        var builder = ErrorBuilder.New();

        // Act
        var returned = builder.WithErrorCode(null);
        var error = returned.Build();

        // Assert
        returned.Should().BeSameAs(builder);
        error.Metadata.Should().NotContainKey("ErrorCode");
    }

    [Fact]
    public void WithMessage_ShouldCreateErrorWithNewMessage()
    {
        // Arrange
        var originalError = ErrorBuilder.New().Build();
        var newMessage = "Custom error message";

        // Act
        var result = ErrorBuilder.New().WithMessage(newMessage).Build();

        // Assert
        result.Should().NotBeSameAs(originalError);
        result.Message.Should().Be(newMessage);
        result.Metadata.Should().BeEquivalentTo(originalError.Metadata);
    }

    [Fact]
    public void WithMessage_ShouldPreserveMetadata()
    {
        // Arrange
        var builder = ErrorBuilder.New()
            .WithLayer<ApplicationLayer>()
            .WithErrorCode(StatusCodes.Status500InternalServerError);
        var newMessage = "Updated message";

        // Act
        var result = builder.WithMessage(newMessage).Build();

        // Assert
        result.Message.Should().Be(newMessage);
        result.Metadata.Should().ContainKey("Layer");
        result.Metadata.Should().ContainKey("ErrorCode");
        result.Metadata["Layer"].Should().Be("ApplicationLayer");
        result.Metadata["ErrorCode"].Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void GetErrorCode_ShouldReturnErrorCode_WhenExists()
    {
        // Arrange
        var error = ErrorBuilder.New().WithErrorCode(StatusCodes.Status404NotFound).Build();

        // Act
        var result = error.GetErrorCode();

        // Assert
        result.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void GetErrorCode_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var error = ErrorBuilder.New().Build();

        // Act
        var result = error.GetErrorCode();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetErrorCode_ShouldReturnNull_WhenValueIsNotInt()
    {
        // Arrange
        var error = ErrorBuilder.New().Build();
        error.Metadata.Add("ErrorCode", "not an int");

        // Act
        var result = error.GetErrorCode();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetLayer_ShouldReturnLayer_WhenExists()
    {
        // Arrange
        var error = ErrorBuilder.New().WithLayer<PersistenceLayer>().Build();

        // Act
        var result = error.GetLayer();

        // Assert
        result.Should().Be("PersistenceLayer");
    }

    [Fact]
    public void GetLayer_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var error = ErrorBuilder.New().Build();

        // Act
        var result = error.GetLayer();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetLayer_ShouldReturnNull_WhenValueIsNotString()
    {
        // Arrange
        var error = ErrorBuilder.New().Build();
        error.Metadata.Add("Layer", 123);

        // Act
        var result = error.GetLayer();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetField_ShouldReturnField_WhenExists()
    {
        // Arrange
        var error = ErrorBuilder.New()
            .WithField("testField")
            .Build();

        // Act
        var result = error.GetField();

        // Assert
        result.Should().Be("testField");
    }

    [Fact]
    public void GetField_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var error = ErrorBuilder.New().Build();

        // Act
        var result = error.GetField();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetErrorCodeString_ShouldReturnCode_WhenExists()
    {
        // Arrange
        var error = ErrorBuilder.New()
            .WithErrorCodeString("INVALID_FORMAT")
            .Build();

        // Act
        var result = error.GetErrorCodeString();

        // Assert
        result.Should().Be("INVALID_FORMAT");
    }

    [Fact]
    public void GetRejectedValue_ShouldReturnValue_WhenExists()
    {
        // Arrange
        var rejectedValue = "invalid_value";
        var error = ErrorBuilder.New()
            .WithRejectedValue(rejectedValue)
            .Build();

        // Act
        var result = error.GetRejectedValue();

        // Assert
        result.Should().Be(rejectedValue);
    }

    [Fact]
    public void ChainedCalls_ShouldWorkCorrectly()
    {
        // Act
        var error = ErrorBuilder.New()
            .WithMessage("Chained error")
            .WithLayer<PresentationLayer>()
            .WithErrorCode(StatusCodes.Status422UnprocessableEntity)
            .WithField("testField")
            .WithErrorCodeString("TEST_ERROR")
            .WithRejectedValue("test")
            .Build();

        // Assert
        error.Message.Should().Be("Chained error");
        error.GetLayer().Should().Be("PresentationLayer");
        error.GetErrorCode().Should().Be(StatusCodes.Status422UnprocessableEntity);
        error.GetField().Should().Be("testField");
        error.GetErrorCodeString().Should().Be("TEST_ERROR");
        error.GetRejectedValue().Should().Be("test");
    }

    public static IEnumerable<object?[]> LayerTypes()
    {
        yield return new object[] { typeof(DomainLayer), "DomainLayer" };
        yield return new object[] { typeof(ApplicationLayer), "ApplicationLayer" };
        yield return new object[] { typeof(PersistenceLayer), "PersistenceLayer" };
        yield return new object[] { typeof(InfrastructureLayer), "InfrastructureLayer" };
        yield return new object[] { typeof(PresentationLayer), "PresentationLayer" };
        yield return new object[] { typeof(UtilitiesLayer), "UtilitiesLayer" };
    }

    [Theory]
    [InlineData(StatusCodes.Status200OK)]
    [InlineData(StatusCodes.Status400BadRequest)]
    [InlineData(StatusCodes.Status401Unauthorized)]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status500InternalServerError)]
    public void WithErrorCode_ShouldHandleCommonStatusCodes(int statusCode)
    {
        // Arrange
        var error = ErrorBuilder.New().WithErrorCode(statusCode).Build();

        // Act
        var result = error.GetErrorCode();

        // Assert
        result.Should().Be(statusCode);
    }

    [Fact]
    public void WithField_ShouldAddFieldToMetadata()
    {
        // Arrange
        var error = ErrorBuilder.New()
            .WithField("version")
            .Build();

        // Act
        var result = error.GetField();

        // Assert
        result.Should().Be("version");
    }

    [Fact]
    public void WithErrorCodeString_ShouldAddCodeToMetadata()
    {
        // Arrange
        var error = ErrorBuilder.New()
            .WithErrorCodeString("TOO_LONG")
            .Build();

        // Act
        var result = error.GetErrorCodeString();

        // Assert
        result.Should().Be("TOO_LONG");
    }

    [Fact]
    public void WithRejectedValue_ShouldAddValueToMetadata()
    {
        // Arrange
        var value = 42;
        var error = ErrorBuilder.New()
            .WithRejectedValue(value)
            .Build();

        // Act
        var result = error.GetRejectedValue();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void CompleteValidationError_ShouldContainAllMetadata()
    {
        // Arrange & Act
        var error = ErrorBuilder.New()
            .WithMessage("Version is too long")
            .WithLayer<DomainLayer>()
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField("version")
            .WithErrorCodeString("TOO_LONG")
            .WithRejectedValue("1234567890123456")
            .Build();

        // Assert
        error.Message.Should().Be("Version is too long");
        error.GetLayer().Should().Be("DomainLayer");
        error.GetErrorCode().Should().Be(400);
        error.GetField().Should().Be("version");
        error.GetErrorCodeString().Should().Be("TOO_LONG");
        error.GetRejectedValue().Should().Be("1234567890123456");
    }
}
