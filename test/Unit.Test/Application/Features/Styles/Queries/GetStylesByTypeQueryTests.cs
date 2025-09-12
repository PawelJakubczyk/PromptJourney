using Application.Features.Styles.Queries;
using Application.Features.Styles.Responses;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;
using Moq;

namespace Unit.Test.Application.Features.Styles.Queries;

public class GetStylesByTypeQueryTests
{
    private readonly Mock<IStyleRepository> _mockStyleRepository;
    private readonly GetStylesByType.Handler _handler;

    public GetStylesByTypeQueryTests()
    {
        _mockStyleRepository = new Mock<IStyleRepository>();
        _handler = new GetStylesByType.Handler(_mockStyleRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var query = new GetStylesByType.Query("Abstract");

        var styles = new List<MidjourneyStyle>
        {
            MidjourneyStyle.Create
            (
                StyleName.Create("Abstract Art"),
                StyleType.Create("Abstract"),
                Description.Create("Beautiful abstract art")
            ).Value,
            MidjourneyStyle.Create
            (
                StyleName.Create("Modern Abstract"),
                StyleType.Create("Abstract"),
                Description.Create("Modern abstract style")
            ).Value
        };

        _mockStyleRepository
            .Setup(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()))
            .ReturnsAsync(Result.Ok(styles));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(s => s.Name == "Abstract Art");
        result.Value.Should().Contain(s => s.Name == "Modern Abstract");
        result.Value.Should().AllSatisfy(s => s.Type.Should().Be("Abstract"));

        _mockStyleRepository.Verify(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidStyleType_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetStylesByType.Query(""); // Invalid empty style type

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNoStylesFound_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetStylesByType.Query("NonExistentType");

        _mockStyleRepository
            .Setup(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()))
            .ReturnsAsync(Result.Ok(new List<MidjourneyStyle>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();

        _mockStyleRepository.Verify(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()), Times.Once);
    }

    [Theory]
    [InlineData("Abstract")]
    [InlineData("Realistic")]
    [InlineData("Minimalist")]
    [InlineData("Futuristic")]
    public async Task Handle_WithVariousValidTypes_ShouldReturnSuccess(string styleType)
    {
        // Arrange
        var query = new GetStylesByType.Query(styleType);

        var styles = new List<MidjourneyStyle>
        {
            MidjourneyStyle.Create
            (
                StyleName.Create($"Test {styleType} Style"),
                StyleType.Create(styleType)
            ).Value
        };

        _mockStyleRepository
            .Setup(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()))
            .ReturnsAsync(Result.Ok(styles));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value[0].Type.Should().Be(styleType);
    }

    [Fact]
    public async Task Handle_WithRepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetStylesByType.Query("Abstract");

        _mockStyleRepository
            .Setup(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()))
            .ReturnsAsync(Result.Fail<List<MidjourneyStyle>>("Database error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithStylesContainingTags_ShouldReturnCorrectResponses()
    {
        // Arrange
        var query = new GetStylesByType.Query("Abstract");

        var styleWithTags = MidjourneyStyle.Create
        (
            StyleName.Create("Tagged Style"),
            StyleType.Create("Abstract"),
            Description.Create("Style with tags"),
            [Tag.Create("modern"), Tag.Create("colorful")]
        ).Value;

        var styles = new List<MidjourneyStyle> { styleWithTags };

        _mockStyleRepository
            .Setup(x => x.GetStylesByTypeAsync(It.IsAny<StyleType>()))
            .ReturnsAsync(Result.Ok(styles));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value[0].Tags.Should().Contain("modern");
        result.Value[0].Tags.Should().Contain("colorful");
    }
}