using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Mapping;

namespace Unit.Persistance.Tests.Mapping;

public class ValueObjectsMappingTests
{
    // ======================================================
    // SIMPLE VALUE OBJECTS - CONVERTER TESTS
    // ======================================================

    [Fact]
    public void DefaultValueConverter_ShouldHandleNone_WhenConvertingToDatabase()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DefaultValueConverter();
        var defaultValue = DefaultValue.None;

        // Act
        var dbValue = converter.ConvertToProvider(defaultValue);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void DefaultValueConverter_ShouldConvertToDatabase_WhenValueIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DefaultValueConverter();
        var defaultValue = DefaultValue.Create("100").Value;

        // Act
        var dbValue = converter.ConvertToProvider(defaultValue) as string;

        // Assert
        dbValue.Should().Be("100");
    }

    [Fact]
    public void DefaultValueConverter_ShouldConvertFromDatabase_WhenValueIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DefaultValueConverter();

        // Act
        var defaultValue = converter.ConvertFromProvider("100") as DefaultValue;

        // Assert
        defaultValue.Should().NotBeNull();
        defaultValue!.Value.Should().Be("100");
    }

    [Fact]
    public void DefaultValueConverter_ShouldReturnNull_WhenDatabaseValueIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DefaultValueConverter();

        // Act
        var defaultValue = converter.ConvertFromProvider(null);

        // Assert
        defaultValue.Should().BeNull();
    }

    [Fact]
    public void DescriptionConverter_ShouldHandleNone_WhenConvertingToDatabase()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DescriptionConverter();
        var description = Description.None;

        // Act
        var dbValue = converter.ConvertToProvider(description);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void DescriptionConverter_ShouldConvertToDatabase_WhenValueIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DescriptionConverter();
        var description = Description.Create("Test description").Value;

        // Act
        var dbValue = converter.ConvertToProvider(description) as string;

        // Assert
        dbValue.Should().Be("Test description");
    }

    [Fact]
    public void DescriptionConverter_ShouldConvertFromDatabase_WhenValueIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DescriptionConverter();

        // Act
        var description = converter.ConvertFromProvider("Test description") as Description;

        // Assert
        description.Should().NotBeNull();
        description!.Value.Should().Be("Test description");
    }

    [Fact]
    public void DescriptionConverter_ShouldReturnNull_WhenDatabaseValueIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.DescriptionConverter();

        // Act
        var description = converter.ConvertFromProvider(null);

        // Assert
        description.Should().BeNull();
    }

    [Fact]
    public void ModelVersionConverter_ShouldConvertToDatabase_WhenVersionIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ModelVersionConverter();
        var version = ModelVersion.Create("6.1").Value;

        // Act
        var dbValue = converter.ConvertToProvider(version) as string;

        // Assert
        dbValue.Should().Be("6.1");
    }

    [Fact]
    public void ModelVersionConverter_ShouldConvertFromDatabase_WhenVersionIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ModelVersionConverter();

        // Act
        var version = converter.ConvertFromProvider("6.1") as ModelVersion;

        // Assert
        version.Should().NotBeNull();
        version!.Value.Should().Be("6.1");
    }

    [Fact]
    public void ParamConverter_ShouldConvertToDatabase_WhenParamIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamConverter();
        var param = Param.Create("--v 1.2").Value;

        // Act
        var dbValue = converter.ConvertToProvider(param) as string;

        // Assert
        dbValue.Should().Be("--v 1.2");
    }

    [Fact]
    public void ParamConverter_ShouldConvertFromDatabase_WhenParamIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamConverter();

        // Act
        var param = converter.ConvertFromProvider("--v 1.2") as Param;

        // Assert
        param.Should().NotBeNull();
        param!.Value.Should().Be("--v 1.2");
    }

    [Fact]
    public void StyleNameConverter_ShouldConvertToDatabase_WhenStyleNameIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.StyleNameConverter();
        var styleName = StyleName.Create("fantasy").Value;

        // Act
        var dbValue = converter.ConvertToProvider(styleName) as string;

        // Assert
        dbValue.Should().Be("fantasy");
    }

    [Fact]
    public void StyleNameConverter_ShouldConvertFromDatabase_WhenStyleNameIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.StyleNameConverter();

        // Act
        var styleName = converter.ConvertFromProvider("fantasy") as StyleName;

        // Assert
        styleName.Should().NotBeNull();
        styleName!.Value.Should().Be("fantasy");
    }

    [Fact]
    public void StyleTypeConverter_ShouldConvertToDatabase_WhenStyleTypeIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.StyleTypeConverter();
        var styleType = StyleType.Create("Custom").Value;

        // Act
        var dbValue = converter.ConvertToProvider(styleType) as string;

        // Assert
        dbValue.Should().Be("Custom");
    }

    [Fact]
    public void StyleTypeConverter_ShouldConvertFromDatabase_WhenStyleTypeIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.StyleTypeConverter();

        // Act
        var styleType = converter.ConvertFromProvider("Custom") as StyleType;

        // Assert
        styleType.Should().NotBeNull();
        styleType!.Value.Should().Be("Custom");
    }

    [Fact]
    public void PropertyNameConverter_ShouldConvertToDatabase_WhenPropertyNameIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.PropertyNameConverter();
        var propertyName = PropertyName.Create("chaos").Value;

        // Act
        var dbValue = converter.ConvertToProvider(propertyName) as string;

        // Assert
        dbValue.Should().Be("chaos");
    }

    [Fact]
    public void PropertyNameConverter_ShouldConvertFromDatabase_WhenPropertyNameIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.PropertyNameConverter();

        // Act
        var propertyName = converter.ConvertFromProvider("chaos") as PropertyName;

        // Assert
        propertyName.Should().NotBeNull();
        propertyName!.Value.Should().Be("chaos");
    }

    [Fact]
    public void ExampleLinkConverter_ShouldConvertToDatabase_WhenLinkIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.LinkConverter();
        var link = ExampleLink.Create("https://example.com/image.png").Value;

        // Act
        var dbValue = converter.ConvertToProvider(link) as string;

        // Assert
        dbValue.Should().Be("https://example.com/image.png");
    }

    [Fact]
    public void ExampleLinkConverter_ShouldConvertFromDatabase_WhenLinkIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.LinkConverter();

        // Act
        var link = converter.ConvertFromProvider("https://example.com/image.png") as ExampleLink;

        // Assert
        link.Should().NotBeNull();
        link!.Value.Should().Be("https://example.com/image.png");
    }

    [Fact]
    public void MinValueConverter_ShouldHandleNone_WhenConvertingToDatabase()
    {
        // Arrange
        var converter = new ValueObjectsMapping.MinValueConverter();
        var minValue = MinValue.None;

        // Act
        var dbValue = converter.ConvertToProvider(minValue);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void MinValueConverter_ShouldConvertToDatabase_WhenValueIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.MinValueConverter();
        var minValue = MinValue.Create("0").Value;

        // Act
        var dbValue = converter.ConvertToProvider(minValue) as string;

        // Assert
        dbValue.Should().Be("0");
    }

    [Fact]
    public void MaxValueConverter_ShouldHandleNone_WhenConvertingToDatabase()
    {
        // Arrange
        var converter = new ValueObjectsMapping.MaxValueConverter();
        var maxValue = MaxValue.None;

        // Act
        var dbValue = converter.ConvertToProvider(maxValue);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void MaxValueConverter_ShouldConvertToDatabase_WhenValueIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.MaxValueConverter();
        var maxValue = MaxValue.Create("1000").Value;

        // Act
        var dbValue = converter.ConvertToProvider(maxValue) as string;

        // Assert
        dbValue.Should().Be("1000");
    }

    // ======================================================
    // SIMPLE VALUE OBJECTS - COMPARER TESTS
    // ======================================================

    [Fact]
    public void DefaultValueComparer_ShouldReturnTrue_WhenValuesAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DefaultValueComparer();
        var value1 = DefaultValue.Create("100").Value;
        var value2 = DefaultValue.Create("100").Value;

        // Act
        var areEqual = comparer.Equals(value1, value2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void DefaultValueComparer_ShouldReturnTrue_WhenBothAreNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DefaultValueComparer();

        // Act
        var areEqual = comparer.Equals(DefaultValue.None, DefaultValue.None);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void DefaultValueComparer_ShouldReturnFalse_WhenValuesAreDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DefaultValueComparer();
        var value1 = DefaultValue.Create("100").Value;
        var value2 = DefaultValue.Create("200").Value;

        // Act
        var areEqual = comparer.Equals(value1, value2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void DefaultValueComparer_ShouldGenerateZeroHashCode_WhenValueIsNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DefaultValueComparer();

        // Act
        var hash = comparer.GetHashCode(DefaultValue.None);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void DefaultValueComparer_ShouldCreateSnapshot_WhenValueIsValid()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DefaultValueComparer();
        var original = DefaultValue.Create("100").Value;

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.Should().NotBeSameAs(original);
        comparer.Equals(original, snapshot).Should().BeTrue();
    }

    [Fact]
    public void DescriptionComparer_ShouldReturnTrue_WhenBothAreNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DescriptionComparer();

        // Act
        var areEqual = comparer.Equals(Description.None, Description.None);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void DescriptionComparer_ShouldGenerateZeroHashCode_WhenValueIsNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DescriptionComparer();

        // Act
        var hash = comparer.GetHashCode(Description.None);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void DescriptionComparer_ShouldCreateSnapshot_WhenValueIsValid()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.DescriptionComparer();
        var original = Description.Create("Test description").Value;

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.Should().NotBeSameAs(original);
        comparer.Equals(original, snapshot).Should().BeTrue();
    }

    [Fact]
    public void ModelVersionComparer_ShouldReturnTrue_WhenVersionsAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ModelVersionComparer();
        var version1 = ModelVersion.Create("6.1").Value;
        var version2 = ModelVersion.Create("6.1").Value;

        // Act
        var areEqual = comparer.Equals(version1, version2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ModelVersionComparer_ShouldReturnFalse_WhenVersionsAreDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ModelVersionComparer();
        var version1 = ModelVersion.Create("6.1").Value;
        var version2 = ModelVersion.Create("7.0").Value;

        // Act
        var areEqual = comparer.Equals(version1, version2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void StyleNameComparer_ShouldReturnTrue_WhenNamesAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.StyleNameComparer();
        var name1 = StyleName.Create("fantasy").Value;
        var name2 = StyleName.Create("fantasy").Value;

        // Act
        var areEqual = comparer.Equals(name1, name2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void StyleTypeComparer_ShouldReturnTrue_WhenTypesAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.StyleTypeComparer();
        var type1 = StyleType.Create("Custom").Value;
        var type2 = StyleType.Create("Custom").Value;

        // Act
        var areEqual = comparer.Equals(type1, type2);

        // Assert
        areEqual.Should().BeTrue();
    }

    // ======================================================
    // RELEASE DATE - CONVERTER & COMPARER TESTS
    // ======================================================

    [Fact]
    public void ReleaseDateConverter_ShouldConvertToDatabase_WhenDateIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ReleaseDateConverter();
        var releaseDate = ReleaseDate.Create("2024-03-15").Value;

        // Act
        var dbValue = converter.ConvertToProvider(releaseDate) as DateTimeOffset?;

        // Assert
        dbValue.Should().NotBeNull();
        dbValue!.Value.Date.Should().Be(new DateTime(2024, 3, 15));
    }

    [Fact]
    public void ReleaseDateConverter_ShouldConvertFromDatabase_WhenDateIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ReleaseDateConverter();
        var dbValue = new DateTimeOffset(2024, 3, 15, 0, 0, 0, TimeSpan.Zero);

        // Act
        var releaseDate = converter.ConvertFromProvider(dbValue) as ReleaseDate;

        // Assert
        releaseDate.Should().NotBeNull();
        releaseDate!.Value.Date.Should().Be(new DateTime(2024, 3, 15));
    }

    [Fact]
    public void ReleaseDateConverter_ShouldReturnNull_WhenDatabaseValueIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ReleaseDateConverter();

        // Act
        var releaseDate = converter.ConvertFromProvider(null);

        // Assert
        releaseDate.Should().BeNull();
    }

    [Fact]
    public void ReleaseDateConverter_ShouldReturnNull_WhenReleaseDateIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ReleaseDateConverter();

        // Act
        var dbValue = converter.ConvertToProvider(null);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void ReleaseDateComparer_ShouldReturnTrue_WhenDatesAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ReleaseDateComparer();
        var date1 = ReleaseDate.Create("2024-03-15").Value;
        var date2 = ReleaseDate.Create("2024-03-15").Value;

        // Act
        var areEqual = comparer.Equals(date1, date2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ReleaseDateComparer_ShouldReturnFalse_WhenDatesAreDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ReleaseDateComparer();
        var date1 = ReleaseDate.Create("2024-03-15").Value;
        var date2 = ReleaseDate.Create("2024-03-16").Value;

        // Act
        var areEqual = comparer.Equals(date1, date2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void ReleaseDateComparer_ShouldReturnTrue_WhenBothAreNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ReleaseDateComparer();

        // Act
        var areEqual = comparer.Equals(null, null);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ReleaseDateComparer_ShouldGenerateZeroHashCode_WhenDateIsNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ReleaseDateComparer();

        // Act
        var hash = comparer.GetHashCode(null);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void ReleaseDateComparer_ShouldCreateSnapshot_WhenDateIsValid()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ReleaseDateComparer();
        var original = ReleaseDate.Create("2024-03-15").Value;

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.Should().NotBeSameAs(original);
        comparer.Equals(original, snapshot).Should().BeTrue();
    }

    // ======================================================
    // COMPREHENSIVE ROUND-TRIP TESTS
    // ======================================================

    [Theory]
    [InlineData("100")]
    [InlineData("0")]
    [InlineData("999")]
    public void DefaultValueConverter_ShouldHandleRoundTrip_ForVariousValues(string value)
    {
        // Arrange
        var converter = new ValueObjectsMapping.DefaultValueConverter();
        var original = DefaultValue.Create(value).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string;
        var restored = converter.ConvertFromProvider(dbValue) as DefaultValue;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("Short desc")]
    [InlineData("A much longer description with multiple words")]
    public void DescriptionConverter_ShouldHandleRoundTrip_ForVariousDescriptions(string value)
    {
        // Arrange
        var converter = new ValueObjectsMapping.DescriptionConverter();
        var original = Description.Create(value).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string;
        var restored = converter.ConvertFromProvider(dbValue) as Description;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("6.1")]
    [InlineData("7.0")]
    [InlineData("niji 6")]
    public void ModelVersionConverter_ShouldHandleRoundTrip_ForVariousVersions(string version)
    {
        // Arrange
        var converter = new ValueObjectsMapping.ModelVersionConverter();
        var original = ModelVersion.Create(version).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string;
        var restored = converter.ConvertFromProvider(dbValue) as ModelVersion;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().Be(version);
    }

    [Theory]
    [InlineData("2024-01-15")]
    [InlineData("2025-12-31")]
    public void ReleaseDateConverter_ShouldHandleRoundTrip_ForVariousDates(string dateString)
    {
        // Arrange
        var converter = new ValueObjectsMapping.ReleaseDateConverter();
        var original = ReleaseDate.Create(dateString).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as DateTimeOffset?;
        var restored = converter.ConvertFromProvider(dbValue) as ReleaseDate;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Date.Should().Be(original.Value.Date);
    }

    // ======================================================
    // NULL & EMPTY HANDLING TESTS
    // ======================================================

    [Fact]
    public void AllSimpleConverters_ShouldReturnNull_WhenInputIsNull()
    {
        // Arrange & Act & Assert
        new ValueObjectsMapping.DefaultValueConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.DescriptionConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.ModelVersionConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.ParamConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.StyleNameConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.StyleTypeConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.TagConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.PropertyNameConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.LinkConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.MinValueConverter().ConvertFromProvider(null).Should().BeNull();
        new ValueObjectsMapping.MaxValueConverter().ConvertFromProvider(null).Should().BeNull();
    }

    [Fact]
    public void AllSimpleConverters_ShouldReturnNull_WhenConvertingNull()
    {
        // Arrange & Act & Assert
        new ValueObjectsMapping.DefaultValueConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.DescriptionConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.ModelVersionConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.ParamConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.StyleNameConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.StyleTypeConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.TagConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.PropertyNameConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.LinkConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.MinValueConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.MaxValueConverter().ConvertToProvider(null).Should().BeNull();
        new ValueObjectsMapping.ReleaseDateConverter().ConvertToProvider(null).Should().BeNull();
    }
}