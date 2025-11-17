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
        result.Should().NotBeSameAs(originalError); // different instances
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
    public void GetDetail_ShouldReturnCorrectDictionary_WithAllValues()
    {
        // Arrange
        var error = ErrorBuilder.New()
            .WithMessage("Test error")
            .WithLayer<InfrastructureLayer>()
            .WithErrorCode(StatusCodes.Status403Forbidden)
            .Build();

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
        var error = ErrorBuilder.New().WithMessage("Simple error").Build();

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
        var error = ErrorBuilder.New()
            .WithMessage("Chained error")
            .WithLayer<PresentationLayer>()
            .WithErrorCode(StatusCodes.Status422UnprocessableEntity)
            .Build();

        // Assert
        error.Message.Should().Be("Chained error");
        error.GetLayer().Should().Be("PresentationLayer");
        error.GetErrorCode().Should().Be(StatusCodes.Status422UnprocessableEntity);
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
    [MemberData(nameof(LayerTypes))
    ]
    public void Withlayer_ShouldHandleAllLayerTypes(Type layerType, string expectedName)
    {
        // Arrange
        var builder = ErrorBuilder.New();

        // Act - call the appropriate generic overload based on the provided Type
        var b = layerType == typeof(DomainLayer) ? builder.WithLayer<DomainLayer>()
              : layerType == typeof(ApplicationLayer) ? builder.WithLayer<ApplicationLayer>()
              : layerType == typeof(PersistenceLayer) ? builder.WithLayer<PersistenceLayer>()
              : layerType == typeof(InfrastructureLayer) ? builder.WithLayer<InfrastructureLayer>()
              : layerType == typeof(PresentationLayer) ? builder.WithLayer<PresentationLayer>()
              : builder.WithLayer<UtilitiesLayer>();

        var error = b.Build();

        // Assert
        error.GetLayer().Should().Be(expectedName);
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
}
