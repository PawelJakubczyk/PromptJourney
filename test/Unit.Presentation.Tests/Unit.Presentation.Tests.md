Unit.Presentation.Tests/TEST_CATALOG.md
# Unit.Presentation.Tests - Test Catalog

## Overview
Unit tests for the Presentation layer (controllers) using Moq for dependency mocking and xUnit as the testing framework.

**Project:** Unit.Presentation.Tests  
**Target Framework:** .NET 9  
**C# Version:** 13.0  
**Testing Approach:** Isolated unit testing with mocked dependencies  

---

## 📋 Test Structure

### Base Classes
- **ControllerTestsBase** - Base class for all controller tests with common helpers
- **ExampleLinksControllerTestsBase** - Specific base for ExampleLinks controller tests
- **VersionsControllerTestsBase** - Specific base for Versions controller tests
- **StylesControllerTestsBase** - Specific base for Styles controller tests

### Test Extensions
- **MockSenderExtensions** - Extension methods for Moq ISender setup
- **AssertionExtensions** - Custom FluentAssertions extensions for Result types

---

## 🎯 ExampleLinks Controller Tests

### CheckLinkExistsTests
**Location:** `MoqControlersTests\ExampleLinksMoqControlersTests\CheckLinkExistsTests.cs`  
**Total Tests:** 23

#### Happy Path Tests
- `CheckLinkExists_ReturnsOkWithTrue_WhenLinkExists` - Returns Ok(true) when link exists
- `CheckLinkExists_ReturnsOkWithFalse_WhenLinkDoesNotExist` - Returns Ok(false) when link doesn't exist

#### Input Validation Tests
- `CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsInvalidGuid` - Invalid GUID format
- `CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsEmpty` - Empty link ID
- `CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsWhitespace` - Whitespace as ID
- `CheckLinkExists_ReturnsBadRequest_WhenNullLinkId` - Null as ID
- `CheckLinkExists_ReturnsBadRequest_ForMalformedGuidWithHyphens` - Malformed GUID

#### Theory Tests
- `CheckLinkExists_ReturnsOk_ForVariousValidGuids` - Various valid GUID formats
  - Data: `00000000-0000-0000-0000-000000000001`, `a1b2c3d4-e5f6-7890-1234-567890abcdef`, `FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF`
- `CheckLinkExists_ReturnsBadRequest_ForInvalidInputs` - Various invalid inputs
  - Data: Empty string, whitespace, "not-a-guid", "12345", "invalid-format-12345", "00000000-0000-0000-0000-00000000000g"

#### Error Handling Tests
- `CheckLinkExists_ReturnsBadRequest_WhenDatabaseErrorOccurs` - Database connection failure

#### Technical Tests
- `CheckLinkExists_VerifiesQueryIsCalledWithCorrectParameters` - Query parameter verification
- `CheckLinkExists_HandlesCancellationToken` - Cancellation token handling
- `CheckLinkExists_VerifiesSenderIsCalledOnce` - Sender invocation count verification
- `CheckLinkExists_ReturnsConsistentResults_ForSameLinkId` - Result consistency
- `CheckLinkExists_HandlesCaseInsensitiveGuids` - Case-insensitive GUID handling

---

### AddExampleLinkTests
**Location:** `MoqControlersTests\ExampleLinksMoqControlersTests\AddExampleLinkTests.cs`  
**Total Tests:** 13

#### Happy Path Tests
- `AddExampleLink_ReturnsCreated_WhenLinkAddedSuccessfully` - Successful link creation

#### Validation Tests
- `AddExampleLink_ReturnsBadRequest_WhenLinkFormatIsInvalid` - Invalid URL format
- `AddExampleLink_ReturnsBadRequest_WhenStyleNameIsInvalid` - Invalid style name (too long)
- `AddExampleLink_ReturnsBadRequest_WhenVersionFormatIsInvalid` - Invalid version format
- `AddExampleLink_ReturnsBadRequest_WhenAllFieldsAreEmpty` - All empty fields

#### Conflict Tests
- `AddExampleLink_ReturnsConflict_WhenLinkAlreadyExists` - Duplicate link (409 Conflict)
- `AddExampleLink_ReturnsConflict_WhenStyleDoesNotExist` - Non-existent style (409 Conflict)

#### Not Found Tests
- `AddExampleLink_ReturnsNotFound_WhenVersionDoesNotExist` - Non-existent version (404)
- `AddExampleLink_ReturnsNotFound_WhenBothStyleAndVersionDoNotExist` - Both missing (404)

#### Technical Tests
- `AddExampleLink_VerifiesCommandIsCalledWithCorrectParameters` - Command parameters verification
- `AddExampleLink_HandlesCancellationToken` - Cancellation token handling

#### Theory Tests
- `AddExampleLink_ReturnsCreated_ForVariousValidInputs` - Various valid input combinations
  - Data: Multiple URL/style/version combinations
- `AddExampleLink_ReturnsBadRequest_ForInvalidInputCombinations` - Invalid input combinations
  - Data: Various null and empty field combinations

---

## 📦 Versions Controller Tests

### CreateVersionTests
**Location:** `MoqControlersTests\VersionsMoqControlersTests\CreateVersionTests.cs`  
**Total Tests:** 35

#### Happy Path Tests
- `Create_ReturnsCreated_WhenVersionCreatedSuccessfully` - Successful version creation
- `Create_ReturnsCreated_WithMinimalRequest` - Minimal required fields
- `Create_ReturnsCreated_WithCompleteRequest` - All fields populated
- `Create_ReturnsCreated_WithNullDescription` - Null description
- `Create_ReturnsCreated_WithNullReleaseDate` - Null release date
- `Create_ReturnsCreated_WithNijiVersion` - Niji version creation

#### Version Validation Tests
- `Create_ReturnsBadRequest_WhenVersionIsEmpty` - Empty version
- `Create_ReturnsBadRequest_WhenVersionIsNull` - Null version
- `Create_ReturnsBadRequest_WhenVersionIsWhitespace` - Whitespace version
- `Create_ReturnsBadRequest_WhenVersionExceedsMaxLength` - Version too long (256 chars)
- `Create_ReturnsCreated_WithVersionContainingDash` - Version with dash (e.g., "7.0-beta")

#### Parameter Validation Tests
- `Create_ReturnsBadRequest_WhenParameterIsEmpty` - Empty parameter
- `Create_ReturnsBadRequest_WhenParameterIsNull` - Null parameter
- `Create_ReturnsBadRequest_WhenParameterIsWhitespace` - Whitespace parameter
- `Create_ReturnsBadRequest_WhenParameterExceedsMaxLength` - Parameter too long (256 chars)

#### Conflict Tests
- `Create_ReturnsBadRequest_WhenVersionAlreadyExists` - Duplicate version

#### Description Tests
- `Create_ReturnsCreated_WithLongDescription` - Long description (1000+ chars)
- `Create_ReturnsCreated_WithSpecialCharactersInDescription` - Special characters and emojis

#### Date Tests
- `Create_ReturnsCreated_WithFutureReleaseDate` - Future date (3 months ahead)
- `Create_ReturnsCreated_WithPastReleaseDate` - Past date (1 year ago)

#### Theory Tests
- `Create_ReturnsCreated_ForVariousVersionFormats` - Various version formats
  - Data: "1.0", "2.5", "5.2", "6.0", "niji 5", "niji 6"

#### Technical Tests
- `Create_VerifiesCommandIsCalledWithCorrectParameters` - Command parameter verification
- `Create_HandlesCancellationToken` - Cancellation token handling
- `Create_VerifiesSenderIsCalledOnce` - Sender invocation count
- `Create_RespondsQuickly_ForPerformanceTest` - Performance test (<1 second)

#### Error Handling Tests
- `Create_ReturnsBadRequest_WhenRepositoryThrowsException` - Repository error (500)
- `Create_ReturnsBadRequest_WhenCommandHandlerFails` - Command handler failure

---

## 🎨 Styles Controller Tests

### CheckExistsTests
**Location:** `MoqControlersTests\StylesMoqControlersTests\CheckExistsTests.cs`  
**Total Tests:** 24

#### Happy Path Tests
- `CheckExists_ReturnsOkWithTrue_WhenStyleExists` - Style exists
- `CheckExists_ReturnsOkWithFalse_WhenStyleDoesNotExist` - Style doesn't exist

#### Validation Tests
- `CheckExists_ReturnsBadRequest_WhenStyleNameIsEmpty` - Empty style name
- `CheckExists_ReturnsBadRequest_WhenStyleNameIsWhitespace` - Whitespace style name
- `CheckExists_ReturnsBadRequest_WhenStyleNameIsNull` - Null style name
- `CheckExists_ReturnsBadRequest_WhenStyleNameExceedsMaxLength` - Name too long (256 chars)

#### Case Sensitivity Tests
- `CheckExists_ReturnsOk_WithLowercaseStyleName` - Lowercase name
- `CheckExists_ReturnsOk_WithUppercaseStyleName` - Uppercase name
- `CheckExists_ReturnsOk_WithMixedCaseStyleName` - Mixed case name

#### Special Characters Tests
- `CheckExists_ReturnsOk_WithStyleNameContainingNumbers` - Name with numbers (Style123)
- `CheckExists_ReturnsOk_WithStyleNameContainingHyphen` - Name with hyphen (modern-art)
- `CheckExists_ReturnsOk_WithStyleNameContainingUnderscore` - Name with underscore (modern_art)
- `CheckExists_ReturnsOk_WithStyleNameContainingSpaces` - Name with spaces (Modern Art)

#### Theory Tests
- `CheckExists_ReturnsOk_ForVariousStyleNames` - Various style names with expected results
  - Data: ModernArt, ClassicStyle, AbstractPainting (true), NonExistentStyle, AnotherNonExistent (false)
- `CheckExists_ReturnsCorrectBooleanValues_ForDifferentStyles` - Different styles with boolean results

#### Technical Tests
- `CheckExists_VerifiesQueryIsCalledWithCorrectParameters` - Query parameter verification
- `CheckExists_HandlesCancellationToken` - Cancellation token handling
- `CheckExists_VerifiesSenderIsCalledOnce` - Sender invocation count
- `CheckExists_ReturnsConsistentResults_ForSameStyleName` - Result consistency
- `CheckExists_RespondsQuickly_ForPerformanceTest` - Performance test (<1 second)

#### Error Handling Tests
- `CheckExists_ReturnsBadRequest_WhenDatabaseErrorOccurs` - Database error (500)
- `CheckExists_ReturnsBadRequest_WhenRepositoryThrowsException` - Repository exception
- `CheckExists_ReturnsBadRequest_WhenQueryHandlerFails` - Query handler failure

---

### CreateStyleTests
**Location:** `MoqControlersTests\StylesMoqControlersTests\CreateStyleTests.cs`  
**Total Tests:** 37

#### Happy Path Tests
- `Create_ReturnsCreatedWithStyleName_WhenStyleCreatedSuccessfully` - Successful creation

#### Name Validation Tests
- `Create_ReturnsBadRequest_WhenNameIsEmpty` - Empty name
- `Create_ReturnsBadRequest_WhenNameIsWhitespace` - Whitespace name
- `Create_ReturnsBadRequest_WhenNameIsNull` - Null name
- `Create_ReturnsBadRequest_WhenNameExceedsMaxLength` - Name too long (256 chars)
- `Create_ReturnsCreated_WithSpecialCharactersInName` - Special characters in name

#### Type Validation Tests
- `Create_ReturnsBadRequest_WhenTypeIsEmpty` - Empty type
- `Create_ReturnsBadRequest_WhenTypeIsWhitespace` - Whitespace type
- `Create_ReturnsBadRequest_WhenTypeIsNull` - Null type
- `Create_ReturnsBadRequest_WhenTypeExceedsMaxLength` - Type too long (256 chars)
- `Create_ReturnsBadRequest_WhenBothNameAndTypeAreEmpty` - Both empty

#### Description Validation Tests
- `Create_ReturnsBadRequest_WhenDescriptionExceedsMaxLength` - Description too long (1000 chars)
- `Create_ReturnsCreated_WithNullDescription` - Null description
- `Create_ReturnsCreated_WithLongDescription` - Long but valid description (500 chars)
- `Create_ReturnsCreated_WithSpecialCharactersInDescription` - Special characters

#### Tags Validation Tests
- `Create_ReturnsBadRequest_WhenTagIsEmpty` - Empty tag in list
- `Create_ReturnsBadRequest_WhenTagExceedsMaxLength` - Tag too long (256 chars)
- `Create_ReturnsCreated_WithEmptyTagsList` - Empty tags list
- `Create_ReturnsCreated_WithNullTagsList` - Null tags list
- `Create_ReturnsCreated_WithMultipleTags` - Multiple tags (4 tags)

#### Conflict Tests
- `Create_ReturnsConflict_WhenStyleAlreadyExists` - Duplicate style (409 Conflict)

#### Theory Tests
- `Create_ReturnsCreated_ForVariousValidInputs` - Various valid name/type combinations
  - Data: ModernArt/Custom, ClassicStyle/Preset, AbstractDesign/Modern, MinimalStyle/Artistic

#### Technical Tests
- `Create_VerifiesCommandIsCalledWithCorrectParameters` - Command parameters verification
- `Create_HandlesCancellationToken` - Cancellation token handling
- `Create_VerifiesSenderIsCalledOnce` - Sender invocation count
- `Create_RespondsQuickly_ForPerformanceTest` - Performance test (<1 second)

#### Error Handling Tests
- `Create_ReturnsBadRequest_WhenDatabaseErrorOccurs` - Database error (500)
- `Create_ReturnsBadRequest_WhenRepositoryThrowsException` - Repository exception
- `Create_ReturnsBadRequest_WhenCommandHandlerFails` - Command handler failure

---

### GetAllStylesTests
**Location:** `MoqControlersTests\StylesMoqControlersTests\GetAllStylesTests.cs`  
**Total Tests:** 27

#### Happy Path Tests
- `GetAll_ReturnsOkWithList_WhenStylesExist` - Returns list with 2 styles
- `GetAll_ReturnsOkWithEmptyList_WhenNoStylesExist` - Empty list when no styles

#### Null Handling Tests
- `GetAll_ReturnsOk_WithStylesHavingNullDescriptions` - Styles with null descriptions
- `GetAll_ReturnsOk_WithStylesHavingNullTags` - Styles with null tags
- `GetAll_ReturnsOk_WithStylesHavingEmptyTagsList` - Styles with empty tags list

#### Volume Tests
- `GetAll_ReturnsOk_WithLargeNumberOfStyles` - 100 styles test
- `GetAll_ReturnsOk_WithStylesHavingVariousTypes` - 4 different style types
- `GetAll_ReturnsOk_WithStylesHavingMultipleTags` - Styles with multiple tags (5 tags vs 1 tag)

#### Data Quality Tests
- `GetAll_ReturnsOk_WithStylesHavingLongDescriptions` - Long descriptions (500 chars)
- `GetAll_ReturnsOk_WithStylesHavingSpecialCharacters` - Special characters in data
- `GetAll_ReturnsOk_WithStylesHavingDifferentCases` - UPPERCASE, lowercase, MixedCase

#### Consistency Tests
- `GetAll_ReturnsConsistentResults_WhenCalledMultipleTimes` - Multiple calls return same results

#### Singleton Pattern Tests
- `GetAll_UsesSingletonQuery` - Verifies Query.Singletone usage
- `GetAll_UsesSingletonPattern_VerifiesSameInstance` - Multiple calls use same singleton instance

#### Theory Tests
- `GetAll_ReturnsOk_ForVariousStyleCounts` - Various counts: 1, 5, 10, 50, 100

#### Technical Tests
- `GetAll_VerifiesSenderIsCalledOnce` - Sender invocation count
- `GetAll_HandlesCancellationToken` - Cancellation token handling
- `GetAll_RespondsQuickly_ForPerformanceTest` - Performance test (<1 second)

#### Error Handling Tests
- `GetAll_ReturnsBadRequest_WhenDatabaseErrorOccurs` - Database error (500)
- `GetAll_ReturnsBadRequest_WhenApplicationLayerErrorOccurs` - Application layer error
- `GetAll_ReturnsBadRequest_WhenRepositoryThrowsException` - Repository exception
- `GetAll_ReturnsBadRequest_WhenQueryHandlerFails` - Query handler failure

---

### GetByTypeTests
**Location:** `MoqControlersTests\StylesMoqControlersTests\GetByTypeTests.cs`  
**Total Tests:** 30

#### Happy Path Tests
- `GetByType_ReturnsOkWithList_WhenStylesWithTypeExist` - Returns 2 matching styles
- `GetByType_ReturnsOkWithEmptyList_WhenNoStylesWithTypeExist` - Empty list for non-existent type

#### Validation Tests
- `GetByType_ReturnsBadRequest_WhenTypeIsEmpty` - Empty type
- `GetByType_ReturnsBadRequest_WhenTypeIsNull` - Null type
- `GetByType_ReturnsBadRequest_WhenTypeIsWhitespace` - Whitespace type
- `GetByType_ReturnsBadRequest_WhenTypeExceedsMaxLength` - Type too long (256 chars)

#### Null Handling Tests
- `GetByType_ReturnsOk_WithStylesHavingNullTags` - Styles with null tags
- `GetByType_ReturnsOk_WithStylesHavingNullDescription` - Styles with null descriptions
- `GetByType_ReturnsOk_WithStylesHavingEmptyTags` - Styles with empty tags list

#### Volume Tests
- `GetByType_ReturnsOk_WithManyMatchingStyles` - 10 matching styles

#### Case Sensitivity Tests
- `GetByType_ReturnsOk_WithCaseInsensitiveStyleType` - Lowercase type matching
- `GetByType_ReturnsOk_WithMixedCaseStyleType` - Mixed case type (MoDeRn-ArT)

#### Special Characters Tests
- `GetByType_ReturnsOk_WithStyleTypeContainingNumbers` - Type with numbers (Modern2024)
- `GetByType_ReturnsOk_WithStyleTypeContainingHyphen` - Type with hyphen (Modern-Art)
- `GetByType_ReturnsOk_WithStyleTypeContainingUnderscore` - Type with underscore (Modern_Art)
- `GetByType_ReturnsOk_WithLongStyleType` - Long type name (100 chars)

#### Tags Tests
- `GetByType_ReturnsOk_WithStylesHavingMultipleTags` - Styles with 4 tags
- `GetByType_ReturnsOk_WithStylesHavingDifferentDescriptions` - Varied description lengths

#### Theory Tests
- `GetByType_ReturnsOk_ForVariousStyleTypes` - Various type names
  - Data: Custom, Abstract, Realistic, Minimalist, Traditional, Contemporary

#### Consistency Tests
- `GetByType_ReturnsConsistentResults_ForSameStyleType` - Multiple calls return same results

#### Technical Tests
- `GetByType_VerifiesQueryIsCalledWithCorrectParameters` - Query parameters verification
- `GetByType_HandlesCancellationToken` - Cancellation token handling
- `GetByType_VerifiesSenderIsCalledOnce` - Sender invocation count
- `GetByType_RespondsQuickly_ForPerformanceTest` - Performance test (<1 second)

#### Error Handling Tests
- `GetByType_ReturnsBadRequest_WhenRepositoryThrowsException` - Repository error (500)
- `GetByType_ReturnsBadRequest_WhenQueryHandlerFails` - Query handler failure

---

## 📊 Statistics

### Total Test Count: **216 tests**

#### By Controller
- **ExampleLinks Controller:** 36 tests
  - CheckLinkExists: 23 tests
  - AddExampleLink: 13 tests
- **Versions Controller:** 35 tests
  - CreateVersion: 35 tests
- **Styles Controller:** 118 tests
  - CheckExists: 24 tests
  - CreateStyle: 37 tests
  - GetAllStyles: 27 tests
  - GetByType: 30 tests

#### By Category
- **Happy Path Tests:** ~30 tests
- **Validation Tests:** ~65 tests
- **Error Handling Tests:** ~25 tests
- **Theory Tests:** ~20 tests
- **Technical Tests:** ~50 tests
- **Edge Cases Tests:** ~26 tests

### Test Coverage Breakdown

#### HTTP Status Codes Tested
- ✅ 200 OK
- ✅ 201 Created
- ✅ 400 Bad Request
- ✅ 404 Not Found
- ✅ 409 Conflict
- ✅ 500 Internal Server Error

#### Validation Scenarios Tested
- ✅ Null values
- ✅ Empty strings
- ✅ Whitespace strings
- ✅ Maximum length violations
- ✅ Invalid formats
- ✅ Special characters
- ✅ Case sensitivity

---

## 🏗️ Test Patterns

### AAA Pattern (Arrange-Act-Assert)
All tests follow the AAA pattern: