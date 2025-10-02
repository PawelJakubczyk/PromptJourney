using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;

namespace Unit.Utilities.Tests.Extensions;

public class ErrorFactoryTests
{
    [Fact]
    public void Create_ShouldReturnErrorWithDefaultMessage()
    {
        // Act
        var error = ErrorFactory.Create();

        // Assert
        error.Should().NotBeNull();
        error.Message.Should().Be("An error occurred");
        error.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Withlayer_ShouldAddLayerToMetadata()
    {
        // Arrange
        var error = ErrorFactory.Create();
        var layerType = typeof(DomainLayer);

        // Act
        var result = error.Withlayer(layerType);

        // Assert
        result.Should().BeSameAs(error); // Should return the same instance
        result.Metadata.Should().ContainKey("Layer");
        result.Metadata["Layer"].Should().Be("DomainLayer");
    }

    [Fact]
    public void WithErrorCode_ShouldAddErrorCodeToMetadata()
    {
        // Arrange
        var error = ErrorFactory.Create();
        var errorCode = StatusCodes.Status400BadRequest;

        // Act
        var result = error.WithErrorCode(errorCode);

        // Assert
        result.Should().BeSameAs(error);
        result.Metadata.Should().ContainKey("ErrorCode");
        result.Metadata["ErrorCode"].Should().Be(errorCode);
    }

    [Fact]
    public void WithErrorCode_WithNullValue_ShouldAddNullToMetadata()
    {
        // Arrange
        var error = ErrorFactory.Create();

        // Act
        var result = error.WithErrorCode(null);

        // Assert
        result.Should().BeSameAs(error);
        result.Metadata.Should().ContainKey("ErrorCode");
        result.Metadata["ErrorCode"].Should().BeNull();
    }

    [Fact]
    public void WithMessage_ShouldCreateNewErrorWithMessage()
    {
        // Arrange
        var originalError = ErrorFactory.Create();
        var newMessage = "Custom error message";

        // Act
        var result = originalError.WithMessage(newMessage);

        // Assert
        result.Should().NotBeSameAs(originalError); // Should return new instance
        result.Message.Should().Be(newMessage);
        result.Metadata.Should().BeEquivalentTo(originalError.Metadata);
    }

    [Fact]
    public void WithMessage_ShouldPreserveMetadata()
    {
        // Arrange
        var error = ErrorFactory.Create()
            .Withlayer(typeof(ApplicationLayer))
            .WithErrorCode(StatusCodes.Status500InternalServerError);
        var newMessage = "Updated message";

        // Act
        var result = error.WithMessage(newMessage);

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
        var error = ErrorFactory.Create().WithErrorCode(StatusCodes.Status404NotFound);

        // Act
        var result = error.GetErrorCode();

        // Assert
        result.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void GetErrorCode_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var error = ErrorFactory.Create();

        // Act
        var result = error.GetErrorCode();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetErrorCode_ShouldReturnNull_WhenValueIsNotInt()
    {
        // Arrange
        var error = ErrorFactory.Create();
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
        var error = ErrorFactory.Create().Withlayer(typeof(PersistenceLayer));

        // Act
        var result = error.GetLayer();

        // Assert
        result.Should().Be("PersistenceLayer");
    }

    [Fact]
    public void GetLayer_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var error = ErrorFactory.Create();

        // Act
        var result = error.GetLayer();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetLayer_ShouldReturnNull_WhenValueIsNotString()
    {
        // Arrange
        var error = ErrorFactory.Create();
        error.Metadata.Add("Layer", 123);

        // Act
        var result = error.GetLayer();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetDetail_ShouldReturnCorrectDictionary_WithAllValues()
    {
        // Arrange
        var error = ErrorFactory.Create()
            .WithMessage("Test error")
            .Withlayer(typeof(InfrastructureLayer))
            .WithErrorCode(StatusCodes.Status403Forbidden);

        // Act
        var result = error.GetDetail();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result["ErrorCode"].Should().Be(StatusCodes.Status403Forbidden.ToString());
        result["Message"].Should().Be("Test error");
        result["Layer"].Should().Be("InfrastructureLayer");
    }

    [Fact]
    public void GetDetail_ShouldReturnDefaults_WhenValuesNotSet()
    {
        // Arrange
        var error = ErrorFactory.Create().WithMessage("Simple error");

        // Act
        var result = error.GetDetail();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result["ErrorCode"].Should().Be(StatusCodes.Status500InternalServerError.ToString());
        result["Message"].Should().Be("Simple error");
        result["Layer"].Should().Be("UnknownLayer");
    }

    [Fact]
    public void ChainedCalls_ShouldWorkCorrectly()
    {
        // Act
        var error = ErrorFactory.Create()
            .WithMessage("Chained error")
            .Withlayer(typeof(PresentationLayer))
            .WithErrorCode(StatusCodes.Status422UnprocessableEntity);

        // Assert
        error.Message.Should().Be("Chained error");
        error.GetLayer().Should().Be("PresentationLayer");
        error.GetErrorCode().Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Theory]
    [InlineData(typeof(DomainLayer), "DomainLayer")]
    [InlineData(typeof(ApplicationLayer), "ApplicationLayer")]
    [InlineData(typeof(PersistenceLayer), "PersistenceLayer")]
    [InlineData(typeof(InfrastructureLayer), "InfrastructureLayer")]
    [InlineData(typeof(PresentationLayer), "PresentationLayer")]
    [InlineData(typeof(UtilitiesLayer), "UtilitiesLayer")]
    public void Withlayer_ShouldHandleAllLayerTypes(Type layerType, string expectedName)
    {
        // Arrange
        var error = ErrorFactory.Create();

        // Act
        var result = error.Withlayer(layerType);

        // Assert
        result.GetLayer().Should().Be(expectedName);
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
        var error = ErrorFactory.Create();

        // Act
        var result = error.WithErrorCode(statusCode);

        // Assert
        result.GetErrorCode().Should().Be(statusCode);
    }
}