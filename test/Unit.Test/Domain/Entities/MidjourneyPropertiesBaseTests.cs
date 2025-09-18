using Domain.Entities;
using Domain.ValueObjects;

namespace Unit.Test.Domain.Entities;

public class MidjourneyPropertiesBaseTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var parametersResults = new List<Result<Param>>
        {
            Param.Create("--ar"),
            Param.Create("--aspect")
        };
        var defaultValueResult = DefaultValue.Create("1:1");
        var minValueResult = MinValue.Create("1:3");
        var maxValueResult = MaxValue.Create("3:1");
        var descriptionResult = Description.Create("Aspect ratio parameter");

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            parametersResults,
            defaultValueResult,
            minValueResult,
            maxValueResult,
            descriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be("aspect");
        result.Value.Version.Value.Should().Be("6.0");
        result.Value.Parameters.Should().NotBeNull();
        result.Value.Parameters.Should().HaveCount(2);
        result.Value.Parameters.Should().Contain(p => p.Value == "--ar");
        result.Value.Parameters.Should().Contain(p => p.Value == "--aspect");
        result.Value.DefaultValue?.Value.Should().Be("1:1");
        result.Value.MinValue?.Value.Should().Be("1:3");
        result.Value.MaxValue?.Value.Should().Be("3:1");
        result.Value.Description?.Value.Should().Be("Aspect ratio parameter");
    }

    [Fact]
    public void Create_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("quality");
        var versionResult = ModelVersion.Create("5.1");

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be("quality");
        result.Value.Version.Value.Should().Be("5.1");
        result.Value.Parameters.Should().NotBeNull();
        result.Value.Parameters.Should().BeEmpty();
        result.Value.DefaultValue.Should().BeNull();
        result.Value.MinValue.Should().BeNull();
        result.Value.MaxValue.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullOptionalParameters_ShouldReturnSuccess()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("stylize");
        var versionResult = ModelVersion.Create("niji 6");

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            null, // parameters
            null, // defaultValue
            null, // minValue
            null, // maxValue
            null  // description
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be("stylize");
        result.Value.Version.Value.Should().Be("niji 6");
        result.Value.Parameters.Should().NotBeNull();
        result.Value.Parameters.Should().BeEmpty();
        result.Value.DefaultValue.Should().BeNull();
        result.Value.MinValue.Should().BeNull();
        result.Value.MaxValue.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyParametersList_ShouldReturnSuccess()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("chaos");
        var versionResult = ModelVersion.Create("6.0");
        var emptyParametersResults = new List<Result<Param>>();

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            emptyParametersResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Parameters.Should().NotBeNull();
        result.Value.Parameters.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithInvalidPropertyName_ShouldReturnFailure()
    {
        // Arrange
        var invalidPropertyNameResult = PropertyName.Create("");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            invalidPropertyNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidVersion_ShouldReturnFailure()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var invalidVersionResult = ModelVersion.Create("invalid_version");

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            invalidVersionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidParameter_ShouldReturnFailure()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var parametersResults = new List<Result<Param>>
        {
            Param.Create("--ar"),
            Param.Create("") // Invalid empty parameter
        };

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            parametersResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidDefaultValue_ShouldReturnFailure()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var invalidDefaultValueResult = DefaultValue.Create(""); // Invalid empty value

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            null,
            invalidDefaultValueResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidMinValue_ShouldReturnFailure()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var invalidMinValueResult = MinValue.Create("   "); // Invalid whitespace

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            null,
            null,
            invalidMinValueResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidMaxValue_ShouldReturnFailure()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var invalidMaxValueResult = MaxValue.Create("\t"); // Invalid whitespace

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            null,
            null,
            null,
            invalidMaxValueResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidDescription_ShouldReturnFailure()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var invalidDescriptionResult = Description.Create(""); // Invalid empty description

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            null,
            null,
            null,
            null,
            invalidDescriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithMultipleInvalidInputs_ShouldReturnFailureWithMultipleErrors()
    {
        // Arrange
        var invalidPropertyNameResult = PropertyName.Create(null);
        var invalidVersionResult = ModelVersion.Create("");
        var invalidParametersResults = new List<Result<Param>>
        {
            Param.Create(null)
        };

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            invalidPropertyNameResult,
            invalidVersionResult,
            invalidParametersResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCountGreaterOrEqualTo(3); // PropertyName, Version, and Parameter errors
    }

    [Theory]
    [InlineData("aspect", "6.0", "--ar")]
    [InlineData("quality", "5.1", "--q")]
    [InlineData("stylize", "niji 6", "--s")]
    [InlineData("chaos", "6.0", "--chaos")]
    [InlineData("weird", "6.0", "--weird")]
    public async Task Create_WithVariousValidProperties_ShouldReturnSuccess(string propertyName, string version, string parameter)
    {
        // Arrange
        var propertyNameResult = PropertyName.Create(propertyName);
        var versionResult = ModelVersion.Create(version);
        var parametersResults = new List<Result<Param>>
        {
            Param.Create(parameter)
        };

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            parametersResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be(propertyName);
        result.Value.Version.Value.Should().Be(version);
        result.Value.Parameters.Should().Contain(p => p.Value == parameter);
    }

    [Fact]
    public void Create_WithMultipleParameters_ShouldReturnSuccess()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var parametersResults = new List<Result<Param>>
        {
            Param.Create("--ar"),
            Param.Create("--aspect"),
            Param.Create("--a")
        };

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            parametersResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Parameters.Should().HaveCount(3);
        result.Value.Parameters.Should().Contain(p => p.Value == "--ar");
        result.Value.Parameters.Should().Contain(p => p.Value == "--aspect");
        result.Value.Parameters.Should().Contain(p => p.Value == "--a");
    }

    [Fact]
    public void Create_WithNullValueObjects_ShouldReturnSuccessWithNullProperties()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("test");
        var versionResult = ModelVersion.Create("6.0");
        var nullDefaultValueResult = DefaultValue.Create(null);
        var nullMinValueResult = MinValue.Create(null);
        var nullMaxValueResult = MaxValue.Create(null);
        var nullDescriptionResult = Description.Create(null);

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            null,
            nullDefaultValueResult,
            nullMinValueResult,
            nullMaxValueResult,
            nullDescriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DefaultValue?.Value.Should().BeNull();
        result.Value.MinValue?.Value.Should().BeNull();
        result.Value.MaxValue?.Value.Should().BeNull();
        result.Value.Description?.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithComplexPropertyConfiguration_ShouldReturnSuccess()
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("aspect");
        var versionResult = ModelVersion.Create("6.0");
        var parametersResults = new List<Result<Param>>
        {
            Param.Create("--ar"),
            Param.Create("--aspect")
        };
        var defaultValueResult = DefaultValue.Create("1:1");
        var minValueResult = MinValue.Create("1:3");
        var maxValueResult = MaxValue.Create("3:1");
        var descriptionResult = Description.Create("Controls the aspect ratio of the generated image. Common values include 1:1 (square), 16:9 (widescreen), 9:16 (portrait)");

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult,
            parametersResults,
            defaultValueResult,
            minValueResult,
            maxValueResult,
            descriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        
        // Verify all properties are set correctly
        result.Value.PropertyName.Value.Should().Be("aspect");
        result.Value.Version.Value.Should().Be("6.0");
        result.Value.Parameters.Should().HaveCount(2);
        result.Value.DefaultValue?.Value.Should().Be("1:1");
        result.Value.MinValue?.Value.Should().Be("1:3");
        result.Value.MaxValue?.Value.Should().Be("3:1");
        result.Value.Description?.Value.Should().StartWith("Controls the aspect ratio");
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("5.1")]
    [InlineData("6.0")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public void Create_WithDifferentVersions_ShouldReturnSuccess(string version)
    {
        // Arrange
        var propertyNameResult = PropertyName.Create("quality");
        var versionResult = ModelVersion.Create(version);

        // Act
        var result = MidjourneyPropertiesBase.Create
        (
            propertyNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(version);
    }
}