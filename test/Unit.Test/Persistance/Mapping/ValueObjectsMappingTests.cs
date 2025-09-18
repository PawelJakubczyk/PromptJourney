using Domain.ValueObjects;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using static Persistence.Mapping.ValueObjectsMapping;
using Xunit;
using Castle.Components.DictionaryAdapter.Xml;

namespace Unit.Test.Persistance.Mapping;

public class ValueObjectsMappingTests
{
    #region Single Value Object Tests

    [Fact]
    public void Converter_ConvertToProvider_WithNullValue_ShouldReturnDefault()
    {
        // Arrange
        var converter = new DescriptionConverter();
        Description? value = null;

        // Act
        var result = converter.ConvertToProvider(value);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Converter_ConvertToProvider_WithValue_ShouldReturnPrimitiveValue()
    {
        // Arrange
        var converter = new DescriptionConverter();
        var description = Description.Create("Test Description").Value;

        // Act
        var result = converter.ConvertToProvider(description);

        // Assert
        result.Should().Be("Test Description");
    }

    [Fact]
    public void Converter_ConvertFromProvider_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var converter = new DescriptionConverter();
        string? value = null;

        // Act
        var result = converter.ConvertFromProvider(value);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Converter_ConvertFromProvider_WithValue_ShouldReturnValueObject()
    {
        // Arrange
        var converter = new DescriptionConverter();
        var value = "Test Description";

        // Act
        var func = converter.ConvertFromProviderExpression.Compile();  // Func<string?, Description?>
        var result = func(value);

        // Assert
        result.Should().NotBeNull()
              .And.BeOfType<Description>()
              .Which.Value.Should().Be(value);
    }

    [Fact]
    public void Comparer_Equals_WithBothNull_ShouldReturnTrue()
    {
        // Arrange
        var comparer = new DescriptionComparer();
        Description? a = null;
        Description? b = null;

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Comparer_Equals_WithOneNull_ShouldReturnFalse()
    {
        // Arrange
        var comparer = new DescriptionComparer();
        var a = Description.Create("Test").Value;
        Description? b = null;

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Comparer_Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var comparer = new DescriptionComparer();
        var a = Description.Create("Test").Value;
        var b = Description.Create("Test").Value;

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region List Value Object Tests

    [Fact]
    public void ListConverter_ConvertToProvider_WithNullList_ShouldReturnNull()
    {
        // Arrange
        var converter = new TagListConverter();
        List<Tag?>? list = null;

        // Act
        var result = converter.ConvertToProvider(list);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ListConverter_ConvertToProvider_WithEmptyList_ViaExpression_ShouldReturnEmptyArray()
    {
        // Arrange
        var converter = new TagListConverter();
        var list = new List<Tag?>();

        // Act
        var func = converter.ConvertToProviderExpression.Compile(); // Func<List<Tag?>?, string[]?>
        var result = func(list);

        // Assert
        result.Should().NotBeNull()
              .And.BeEmpty();
    }

    [Fact]
    public void ListConverter_ConvertToProvider_WithValues_ShouldReturnArray()
    {
        // Arrange
        var converter = new TagListConverter();
        var list = new List<Tag?>
        {
            Tag.Create("tag1").Value,
            Tag.Create("tag2").Value
        };

        // Act
        var func = converter.ConvertToProviderExpression.Compile(); // Func<List<Tag?>?, string[]?>
        var result = func(list);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("tag1");
        result.Should().Contain("tag2");
    }

    [Fact]
    public void ListComparer_Equals_WithBothNull_ShouldReturnTrue()
    {
        // Arrange
        var comparer = new TagListComparer();
        List<Tag?>? a = null;
        List<Tag?>? b = null;

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ListComparer_Equals_WithOneNull_ShouldReturnFalse()
    {
        // Arrange
        var comparer = new TagListComparer();
        var a = new List<Tag?> { Tag.Create("tag1").Value };
        List<Tag?>? b = null;

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ListComparer_Equals_WithSameLists_ShouldReturnTrue()
    {
        // Arrange
        var comparer = new TagListComparer();
        var a = new List<Tag?> { Tag.Create("tag1").Value, Tag.Create("tag2").Value };
        var b = new List<Tag?> { Tag.Create("tag1").Value, Tag.Create("tag2").Value };

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ListComparer_GetHashCode_WithNull_ShouldReturn0()
    {
        // Arrange
        var comparer = new TagListComparer();
        List<Tag?>? list = null;

        // Act
        var result = comparer.GetHashCode(list);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ListComparer_GetHashCode_WithValues_ShouldReturnConsistentHash()
    {
        // Arrange
        var comparer = new TagListComparer();
        var list1 = new List<Tag?> { Tag.Create("tag1").Value, Tag.Create("tag2").Value };
        var list2 = new List<Tag?> { Tag.Create("tag1").Value, Tag.Create("tag2").Value };

        // Act
        var hash1 = comparer.GetHashCode(list1);
        var hash2 = comparer.GetHashCode(list2);

        // Assert
        hash1.Should().Be(hash2);
    }

    #endregion
}