using Domain.Errors;
using Domain.ValueObjects;

namespace Unit.Test.Domain.Errors;

public class DomainErrorsExtensionsTests
{
    #region If Tests
    [Fact]
    public void If_WhenConditionIsTrue_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var error = new DomainError("Test error");

        // Act
        errors.If(true, error);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Test error");
    }

    [Fact]
    public void If_WhenConditionIsFalse_ShouldNotAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var error = new DomainError("Test error");

        // Act
        errors.If(false, error);

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion

    #region IfNullOrWhitespace Tests
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void IfNullOrWhitespace_WithInvalidInput_ShouldAddError(string? value)
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfNullOrWhitespace<Tag>(value);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be($"Tag: {value} cannot be null or whitespace.");
    }

    [Fact]
    public void IfNullOrWhitespace_WithValidInput_ShouldNotAddError()
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfNullOrWhitespace<Tag>("valid-value");

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion

    #region IfWhitespace Tests
    [Theory]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("   ")]
    public void IfWhitespace_WithWhitespaceOnly_ShouldAddError(string value)
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfWhitespace<Tag>(value);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Tag: cannot be whitespace.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("valid")]
    public void IfWhitespace_WithValidOrNull_ShouldNotAddError(string? value)
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfWhitespace<Tag>(value);

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion

    #region IfNull Tests
    [Fact]
    public void IfNull_WithNullValue_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        object? value = null;

        // Act
        errors.IfNull<Tag>(value);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Tag: cannot be null.");
    }

    [Fact]
    public void IfNull_WithNonNullValue_ShouldNotAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        object value = new();

        // Act
        errors.IfNull<Tag>(value);

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion

    #region IfLengthTooLong Tests
    [Fact]
    public void IfLengthTooLong_WithTooLongValue_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var value = "test";
        const int maxLength = 3;

        // Act
        errors.IfLengthTooLong<Tag>(value, maxLength);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be($"Tag: {value} cannot be longer than {maxLength} characters.");
    }

    [Theory]
    [InlineData("test", 4)]
    [InlineData("test", 5)]
    [InlineData(null, 5)]
    public void IfLengthTooLong_WithValidLength_ShouldNotAddError(string? value, int maxLength)
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfLengthTooLong<Tag>(value, maxLength);

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion

    #region IfLinkFormatInvalid Tests
    [Theory]
    [InlineData("invalid-url")]
    [InlineData("ftp://invalid-scheme.com")]
    [InlineData("not-a-url")]
    public void IfLinkFormatInvalid_WithInvalidUrl_ShouldAddError(string url)
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfLinkFormatInvalid(url);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be($"Invalid URL format: {url}");
    }

    [Theory]
    [InlineData("http://valid-url.com")]
    [InlineData("https://valid-url.com")]
    [InlineData("https://valid-url.com/path?param=value")]
    public void IfLinkFormatInvalid_WithValidUrl_ShouldNotAddError(string url)
    {
        // Arrange
        var errors = new List<DomainError>();

        // Act
        errors.IfLinkFormatInvalid(url);

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion

    #region List Operation Tests
    [Fact]
    public void IfListIsEmpty_WithEmptyList_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var emptyList = new List<Tag>();

        // Act
        errors.IfListIsEmpty(emptyList);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Tag: Cannot be an empty collection.");
    }

    [Fact]
    public void IfDoesNotContain_WhenElementNotInList_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var list = new List<Tag> { Tag.Create("tag1").Value };
        var searchTag = Tag.Create("tag2").Value;

        // Act
        errors.IfDoesNotContain(list, searchTag);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Tag: Collection does not contain the required element.");
    }

    [Fact]
    public void IfTagAlreadyExist_WhenTagExists_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var tag = Tag.Create("tag1").Value;
        var list = new List<Tag> { tag };

        // Act
        errors.IfTagAllredyExist(list, tag);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Contain("already exist in tags");
    }

    [Fact]
    public void IfContain_WhenElementExists_ShouldAddError()
    {
        // Arrange
        var errors = new List<DomainError>();
        var tag = Tag.Create("tag1").Value;
        var list = new List<Tag> { tag };

        // Act
        errors.IfContain(list, tag);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Tag: Collection already contains the element.");
    }
    #endregion

    #region CollectErrors Tests
    [Fact]
    public void CollectErrors_WithFailedResult_ShouldCollectDomainErrors()
    {
        // Arrange
        var errors = new List<DomainError>();
        var domainError = new DomainError("Test domain error");
        var failedResult = Result.Fail<Tag>(domainError);

        // Act
        errors.CollectErrors(failedResult);

        // Assert
        errors.Should().ContainSingle()
              .Which.Message.Should().Be("Test domain error");
    }

    [Fact]
    public void CollectErrors_WithSuccessResult_ShouldNotAddErrors()
    {
        // Arrange
        var errors = new List<DomainError>();
        var successResult = Result.Ok(Tag.Create("tag1").Value);

        // Act
        errors.CollectErrors(successResult);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void CollectErrors_WithNullResult_ShouldNotAddErrors()
    {
        // Arrange
        var errors = new List<DomainError>();
        Result<Tag>? nullResult = null;

        // Act
        errors.CollectErrors(nullResult);

        // Assert
        errors.Should().BeEmpty();
    }
    #endregion
}