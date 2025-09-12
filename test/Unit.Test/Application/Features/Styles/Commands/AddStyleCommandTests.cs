using Application.Features.Styles.Commands;
using Application.Features.Styles.Responses;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;
using Moq;
using Application.Features.Styles.Commands.AddStyle;

namespace Unit.Test.Application.Features.Styles.Commands;

public class AddStyleCommandTests
{
    private readonly Mock<IStyleRepository> _mockStyleRepository;
    private readonly AddStyle.Handler _handler;

    public AddStyleCommandTests()
    {
        _mockStyleRepository = new Mock<IStyleRepository>();
        _handler = new AddStyle.Handler(_mockStyleRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Abstract Art",
            Type: "Abstract",
            Description: "Beautiful abstract art style",
            Tags: ["modern", "colorful"]
        );

        var expectedStyle = MidjourneyStyle.Create
        (
            StyleName.Create("Abstract Art"),
            StyleType.Create("Abstract"),
            Description.Create("Beautiful abstract art style"),
            [Tag.Create("modern"), Tag.Create("colorful")]
        ).Value;

        _mockStyleRepository
            .Setup(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()))
            .ReturnsAsync(Result.Ok(false));

        _mockStyleRepository
            .Setup(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()))
            .ReturnsAsync(Result.Ok(expectedStyle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Abstract Art");
        result.Value.Type.Should().Be("Abstract");
        result.Value.Description.Should().Be("Beautiful abstract art style");
        result.Value.Tags.Should().Contain("modern");
        result.Value.Tags.Should().Contain("colorful");

        _mockStyleRepository.Verify(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()), Times.Once);
        _mockStyleRepository.Verify(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Minimalist",
            Type: "Clean"
        );

        var expectedStyle = MidjourneyStyle.Create
        (
            StyleName.Create("Minimalist"),
            StyleType.Create("Clean")
        ).Value;

        _mockStyleRepository
            .Setup(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()))
            .ReturnsAsync(Result.Ok(false));

        _mockStyleRepository
            .Setup(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()))
            .ReturnsAsync(Result.Ok(expectedStyle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Minimalist");
        result.Value.Type.Should().Be("Clean");
        result.Value.Description.Should().BeNull();
        result.Value.Tags.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithInvalidStyleName_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "", // Invalid empty name
            Type: "Abstract"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidStyleType_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Test Style",
            Type: "" // Invalid empty type
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingStyleName_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Existing Style",
            Type: "Abstract"
        );

        _mockStyleRepository
            .Setup(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()))
            .ReturnsAsync(Result.Ok(true)); // Style already exists

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()), Times.Once);
        _mockStyleRepository.Verify(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithRepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Test Style",
            Type: "Abstract"
        );

        _mockStyleRepository
            .Setup(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()))
            .ReturnsAsync(Result.Ok(false));

        _mockStyleRepository
            .Setup(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()))
            .ReturnsAsync(Result.Fail<MidjourneyStyle>("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()), Times.Once);
    }

    [Theory]
    [InlineData("Abstract Art", "Abstract")]
    [InlineData("Cyberpunk", "Futuristic")]
    [InlineData("Minimalist", "Clean")]
    [InlineData("Photorealistic", "Realistic")]
    public async Task Handle_WithVariousValidStyles_ShouldReturnSuccess(string name, string type)
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: name,
            Type: type
        );

        var expectedStyle = MidjourneyStyle.Create
        (
            StyleName.Create(name),
            StyleType.Create(type)
        ).Value;

        _mockStyleRepository
            .Setup(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()))
            .ReturnsAsync(Result.Ok(false));

        _mockStyleRepository
            .Setup(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()))
            .ReturnsAsync(Result.Ok(expectedStyle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(name);
        result.Value.Type.Should().Be(type);
    }

    [Fact]
    public async Task Handle_WithEmptyTags_ShouldReturnSuccess()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Test Style",
            Type: "Test Type",
            Tags: []
        );

        var expectedStyle = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type")
        ).Value;

        _mockStyleRepository
            .Setup(x => x.CheckStyleExistsAsync(It.IsAny<StyleName>()))
            .ReturnsAsync(Result.Ok(false));

        _mockStyleRepository
            .Setup(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()))
            .ReturnsAsync(Result.Ok(expectedStyle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Tags.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithInvalidTag_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddStyle.Command
        (
            Name: "Test Style",
            Type: "Test Type",
            Tags: ["valid", ""] // One invalid empty tag
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockStyleRepository.Verify(x => x.AddStyleAsync(It.IsAny<MidjourneyStyle>()), Times.Never);
    }
}