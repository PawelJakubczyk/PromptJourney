using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Mapping;

namespace Unit.Persistance.Tests.Mapping;

public class CollectionValueObjectsMappingTests
{
    // ======================================================
    // PARAMS COLLECTION - CONVERTER TESTS
    // ======================================================

    [Fact]
    public void ParamsCollectionConverter_ShouldConvertToDatabase_WhenCollectionIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        var paramsCollection = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(paramsCollection) as string[];

        // Assert
        dbValue.Should().NotBeNull();
        dbValue.Should().HaveCount(2);
        dbValue.Should().Contain("--v 1.2");
        dbValue.Should().Contain("--ar 16:9");
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldConvertFromDatabase_WhenArrayIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        string[] dbValue = ["--v 1.2", "--ar 16:9"];

        // Act
        var paramsCollection = converter.ConvertFromProvider(dbValue) as ParamsCollection;

        // Assert
        paramsCollection.Should().NotBeNull();
        paramsCollection!.Value.Should().HaveCount(2);
        paramsCollection.Value[0].Value.Should().Be("--v 1.2");
        paramsCollection.Value[1].Value.Should().Be("--ar 16:9");
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldReturnNull_WhenCollectionIsNone()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        var paramsCollection = ParamsCollection.None;

        // Act
        var dbValue = converter.ConvertToProvider(paramsCollection);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldReturnNull_WhenCollectionIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();

        // Act
        var dbValue = converter.ConvertToProvider(null);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldReturnNull_WhenDatabaseArrayIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();

        // Act
        var paramsCollection = converter.ConvertFromProvider(null);

        // Assert
        paramsCollection.Should().BeNull();
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldReturnNull_WhenDatabaseArrayIsEmpty()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        string[] dbValue = [];

        // Act
        var paramsCollection = converter.ConvertFromProvider(dbValue) as ParamsCollection;

        // Assert
        paramsCollection.Should().BeNull();
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldHandleSingleElement()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        var paramsCollection = ParamsCollection.Create(["--v 1.2"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(paramsCollection) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as ParamsCollection;

        // Assert
        dbValue.Should().HaveCount(1);
        restored.Should().NotBeNull();
        restored!.Value.Should().HaveCount(1);
        restored.Value[0].Value.Should().Be("--v 1.2");
    }

    [Fact]
    public void ParamsCollectionConverter_ShouldHandleMultipleElements()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        var paramsCollection = ParamsCollection.Create(
            ["--v 1.2", "--ar 16:9", "--s 100", "--q 2"]
        ).Value;

        // Act
        var dbValue = converter.ConvertToProvider(paramsCollection) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as ParamsCollection;

        // Assert
        dbValue.Should().HaveCount(4);
        restored.Should().NotBeNull();
        restored!.Value.Should().HaveCount(4);
        restored.Value.Select(p => p.Value).Should().BeEquivalentTo(
            ["--v 1.2", "--ar 16:9", "--s 100", "--q 2"]
        );
    }

    // ======================================================
    // PARAMS COLLECTION - COMPARER TESTS
    // ======================================================

    [Fact]
    public void ParamsCollectionComparer_ShouldReturnTrue_WhenCollectionsAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection1 = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;
        var collection2 = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldReturnFalse_WhenCollectionsAreDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection1 = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;
        var collection2 = ParamsCollection.Create(["--v 2.0", "--ar 4:3"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldReturnTrue_WhenBothAreNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection1 = ParamsCollection.None;
        var collection2 = ParamsCollection.None;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldReturnTrue_WhenBothAreNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();

        // Act
        var areEqual = comparer.Equals(null, null);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldReturnFalse_WhenOneIsNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection = ParamsCollection.Create(["--v 1.2"]).Value;

        // Act
        var areEqual1 = comparer.Equals(collection, null);
        var areEqual2 = comparer.Equals(null, collection);

        // Assert
        areEqual1.Should().BeFalse();
        areEqual2.Should().BeFalse();
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldReturnFalse_WhenOrderIsDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection1 = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;
        var collection2 = ParamsCollection.Create(["--ar 16:9", "--v 1.2"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeFalse(); // Order matters in SequenceEqual
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldGenerateSameHashCode_WhenCollectionsAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection1 = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;
        var collection2 = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;

        // Act
        var hash1 = comparer.GetHashCode(collection1);
        var hash2 = comparer.GetHashCode(collection2);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldGenerateDifferentHashCode_WhenCollectionsAreDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var collection1 = ParamsCollection.Create(["--v 1.2"]).Value;
        var collection2 = ParamsCollection.Create(["--v 2.0"]).Value;

        // Act
        var hash1 = comparer.GetHashCode(collection1);
        var hash2 = comparer.GetHashCode(collection2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldGenerateZeroHashCode_WhenCollectionIsNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();

        // Act
        var hash = comparer.GetHashCode(null);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldGenerateZeroHashCode_WhenCollectionIsNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();

        // Act
        var hash = comparer.GetHashCode(ParamsCollection.None);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldCreateSnapshot_WhenCollectionIsValid()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var original = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.Should().NotBeSameAs(original); // Different instance
        comparer.Equals(original, snapshot).Should().BeTrue(); // But equal values
    }

    // ======================================================
    // TAGS COLLECTION - CONVERTER TESTS
    // ======================================================

    [Fact]
    public void TagsCollectionConverter_ShouldConvertToDatabase_WhenCollectionIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        var tagsCollection = TagsCollection.Create(["fantasy", "magic"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(tagsCollection) as string[];

        // Assert
        dbValue.Should().NotBeNull();
        dbValue.Should().HaveCount(2);
        dbValue.Should().Contain("fantasy");
        dbValue.Should().Contain("magic");
    }

    [Fact]
    public void TagsCollectionConverter_ShouldConvertFromDatabase_WhenArrayIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        string[] dbValue = ["fantasy", "magic"];

        // Act
        var tagsCollection = converter.ConvertFromProvider(dbValue) as TagsCollection;

        // Assert
        tagsCollection.Should().NotBeNull();
        tagsCollection!.Value.Should().HaveCount(2);
        tagsCollection.Value[0].Value.Should().Be("fantasy");
        tagsCollection.Value[1].Value.Should().Be("magic");
    }

    [Fact]
    public void TagsCollectionConverter_ShouldReturnNull_WhenCollectionIsNone()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        var tagsCollection = TagsCollection.None;

        // Act
        var dbValue = converter.ConvertToProvider(tagsCollection);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void TagsCollectionConverter_ShouldReturnNull_WhenCollectionIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();

        // Act
        var dbValue = converter.ConvertToProvider(null);

        // Assert
        dbValue.Should().BeNull();
    }

    [Fact]
    public void TagsCollectionConverter_ShouldReturnNull_WhenDatabaseArrayIsNull()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();

        // Act
        var tagsCollection = converter.ConvertFromProvider(null);

        // Assert
        tagsCollection.Should().BeNull();
    }

    [Fact]
    public void TagsCollectionConverter_ShouldReturnNull_WhenDatabaseArrayIsEmpty()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        string[] dbValue = [];

        // Act
        var tagsCollection = converter.ConvertFromProvider(dbValue) as TagsCollection;

        // Assert
        tagsCollection.Should().BeNull();
    }

    [Fact]
    public void TagsCollectionConverter_ShouldHandleSingleTag()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        var tagsCollection = TagsCollection.Create(["fantasy"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(tagsCollection) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as TagsCollection;

        // Assert
        dbValue.Should().NotBeNull();
        dbValue.Should().HaveCount(1);
        dbValue![0].Should().Be("fantasy");
        restored.Should().NotBeNull();
        restored!.Value.Should().HaveCount(1);
        restored.Value[0].Value.Should().Be("fantasy");
    }

    [Fact]
    public void TagsCollectionConverter_ShouldHandleMultipleTags()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        var tagsCollection = TagsCollection.Create(
            ["fantasy", "magic", "dark", "medieval"]
        ).Value;

        // Act
        var dbValue = converter.ConvertToProvider(tagsCollection) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as TagsCollection;

        // Assert
        dbValue.Should().NotBeNull();
        dbValue.Should().HaveCount(4);
        restored.Should().NotBeNull();
        restored!.Value.Should().HaveCount(4);
        restored.Value.Select(t => t.Value).Should().BeEquivalentTo(
            ["fantasy", "magic", "dark", "medieval"]
        );
    }

    [Fact]
    public void TagsCollectionConverter_ShouldPreserveOrder_WhenConvertingRoundTrip()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        var tagsCollection = TagsCollection.Create(["aaa", "zzz", "mmm"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(tagsCollection) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as TagsCollection;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Select(t => t.Value).Should().ContainInOrder("aaa", "zzz", "mmm");
    }

    // ======================================================
    // TAGS COLLECTION - COMPARER TESTS
    // ======================================================

    [Fact]
    public void TagsCollectionComparer_ShouldReturnTrue_WhenCollectionsAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection1 = TagsCollection.Create(["fantasy", "magic"]).Value;
        var collection2 = TagsCollection.Create(["fantasy", "magic"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnFalse_WhenCollectionsAreDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection1 = TagsCollection.Create(["fantasy", "magic"]).Value;
        var collection2 = TagsCollection.Create(["dark", "medieval"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnTrue_WhenBothAreNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection1 = TagsCollection.None;
        var collection2 = TagsCollection.None;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnTrue_WhenBothAreNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();

        // Act
        var areEqual = comparer.Equals(null, null);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnFalse_WhenOneIsNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection = TagsCollection.Create(["fantasy"]).Value;

        // Act
        var areEqual1 = comparer.Equals(collection, null);
        var areEqual2 = comparer.Equals(null, collection);

        // Assert
        areEqual1.Should().BeFalse();
        areEqual2.Should().BeFalse();
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnFalse_WhenOrderIsDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection1 = TagsCollection.Create(["fantasy", "magic"]).Value;
        var collection2 = TagsCollection.Create(["magic", "fantasy"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeFalse(); // SequenceEqual checks order
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnFalse_WhenCountIsDifferent()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection1 = TagsCollection.Create(["fantasy"]).Value;
        var collection2 = TagsCollection.Create(["fantasy", "magic"]).Value;

        // Act
        var areEqual = comparer.Equals(collection1, collection2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void TagsCollectionComparer_ShouldGenerateSameHashCode_WhenCollectionsAreEqual()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var collection1 = TagsCollection.Create(["fantasy", "magic"]).Value;
        var collection2 = TagsCollection.Create(["fantasy", "magic"]).Value;

        // Act
        var hash1 = comparer.GetHashCode(collection1);
        var hash2 = comparer.GetHashCode(collection2);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void TagsCollectionComparer_ShouldGenerateZeroHashCode_WhenCollectionIsNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();

        // Act
        var hash = comparer.GetHashCode(null);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void TagsCollectionComparer_ShouldGenerateZeroHashCode_WhenCollectionIsNone()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();

        // Act
        var hash = comparer.GetHashCode(TagsCollection.None);

        // Assert
        hash.Should().Be(0);
    }

    [Fact]
    public void TagsCollectionComparer_ShouldCreateSnapshot_WhenCollectionIsValid()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();
        var original = TagsCollection.Create(["fantasy", "magic"]).Value;

        // Act
        var snapshot = comparer.Snapshot(original);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot.Should().NotBeSameAs(original); // Different instance (deep copy)
        comparer.Equals(original, snapshot).Should().BeTrue(); // But equal values
    }

    [Fact]
    public void TagsCollectionComparer_ShouldReturnNull_WhenSnapshotOfNull()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.TagsCollectionComparer();

        // Act
        var snapshot = comparer.Snapshot(null);

        // Assert
        snapshot.Should().BeNull();
    }

    // ======================================================
    // EDGE CASES & INTEGRATION TESTS
    // ======================================================

    [Fact]
    public void ParamsCollectionConverter_ShouldHandleRoundTrip_WhenCollectionIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamsCollectionConverter();
        var original = ParamsCollection.Create(["--v 6.1", "--ar 16:9", "--s 750"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as ParamsCollection;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().HaveCount(3);

        for (int i = 0; i < original.Value.Count; i++)
        {
            restored.Value[i].Value.Should().Be(original.Value[i].Value);
        }
    }

    [Fact]
    public void TagsCollectionConverter_ShouldHandleRoundTrip_WhenCollectionIsValid()
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagsCollectionConverter();
        var original = TagsCollection.Create(["fantasy", "magic", "dark"]).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string[];
        var restored = converter.ConvertFromProvider(dbValue) as TagsCollection;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().HaveCount(3);

        for (int i = 0; i < original.Value.Count; i++)
        {
            restored.Value[i].Value.Should().Be(original.Value[i].Value);
        }
    }

    [Fact]
    public void ParamsCollectionComparer_ShouldDetectChanges_WhenCollectionIsModified()
    {
        // Arrange
        var comparer = new ValueObjectsMapping.ParamsCollectionComparer();
        var original = ParamsCollection.Create(["--v 1.2"]).Value;
        var modified = ParamsCollection.Create(["--v 1.2", "--ar 16:9"]).Value;

        // Act
        var areEqual = comparer.Equals(original, modified);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Theory]
    [InlineData("--v 1.2")]
    [InlineData("--ar 16:9")]
    [InlineData("--s 100")]
    [InlineData("--q 2")]
    public void ParamConverter_ShouldHandleRoundTrip_ForVariousParams(string paramValue)
    {
        // Arrange
        var converter = new ValueObjectsMapping.ParamConverter();
        var original = Param.Create(paramValue).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string;
        var restored = converter.ConvertFromProvider(dbValue) as Param;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().Be(paramValue);
    }

    [Theory]
    [InlineData("fantasy")]
    [InlineData("magic")]
    [InlineData("dark")]
    [InlineData("medieval")]
    public void TagConverter_ShouldHandleRoundTrip_ForVariousTags(string tagValue)
    {
        // Arrange
        var converter = new ValueObjectsMapping.TagConverter();
        var original = Tag.Create(tagValue).Value;

        // Act
        var dbValue = converter.ConvertToProvider(original) as string;
        var restored = converter.ConvertFromProvider(dbValue) as Tag;

        // Assert
        restored.Should().NotBeNull();
        restored!.Value.Should().Be(tagValue);
    }
}