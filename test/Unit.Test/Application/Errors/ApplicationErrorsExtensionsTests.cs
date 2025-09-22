//using System.Reflection;
//using Application.Features.Properties;
//using Application.Extension;

//namespace Unit.Test.Application.Errors;

//public class ApplicationErrorsExtensionsTests
//{
//    [Fact]
//    public void CreateValidationErrorIfAny_WithNoErrors_ReturnsNull()
//    {
//        // Arrange
//        var appErrors = new List<global::Application.Extension.Error>();
//        var domainErrors = new List<DomainError>();

//        // Act
//        var result = ErrorsExtensions.CreateValidationErrorIfAny<string>(appErrors, domainErrors);

//        // Assert
//        result.Should().BeNull();
//    }

//    [Fact]
//    public void CreateValidationErrorIfAny_WithAppError_ReturnsResultWithMetadata()
//    {
//        // Arrange
//        var appErrors = new List<global::Application.Extension.Error> { new global::Application.Extension.Error("app error") };
//        var domainErrors = new List<DomainError>();

//        // Act
//        var result = ErrorsExtensions.CreateValidationErrorIfAny<string>(appErrors, domainErrors);

//        // Assert
//        result.Should().NotBeNull();
//        result!.IsFailed.Should().BeTrue();
//        result.Errors[0].Message.Should().Be("Validation failed");
//        result.Errors[0].Metadata.Should().ContainKey("Application Errors");
//    }

//    [Fact]
//    public void CreateValidationErrorIfAny_WithDomainError_ReturnsResultWithMetadata()
//    {
//        // Arrange
//        var appErrors = new List<global::Application.Extension.Error>();
//        var domainErrors = new List<DomainError> { new DomainError("domain error") };

//        // Act
//        var result = ErrorsExtensions.CreateValidationErrorIfAny<string>(appErrors, domainErrors);

//        // Assert
//        result.Should().NotBeNull();
//        result!.IsFailed.Should().BeTrue();
//        result.Errors[0].Message.Should().Be("Validation failed");
//        result.Errors[0].Metadata.Should().ContainKey("Domain Errors");
//    }

//    [Fact]
//    public void CreateValidationErrorIfAny_WithOnlyDomainErrorsList_ReturnsResult()
//    {
//        // Arrange
//        var domainErrors = new List<DomainError> { new DomainError("domain error") };

//        // Act
//        var result = ErrorsExtensions.CreateValidationErrorIfAny<string>(domainErrors);

//        // Assert
//        result.Should().NotBeNull();
//        result!.IsFailed.Should().BeTrue();
//        result.Errors[0].Metadata.Should().ContainKey("Domain Errors");
//    }

//    [Fact]
//    public void CreateValidationErrorIfAny_WithOnlyAppErrorsList_ReturnsResult()
//    {
//        // Arrange
//        var appErrors = new List<global::Application.Extension.Error> { new global::Application.Extension.Error("app error") };

//        // Act
//        var result = ErrorsExtensions.CreateValidationErrorIfAny<string>(appErrors);

//        // Assert
//        result.Should().NotBeNull();
//        result!.IsFailed.Should().BeTrue();
//        result.Errors[0].Metadata.Should().ContainKey("Domain Errors");
//    }

//    [Fact]
//    public void IfHistoryLimitNotGreaterThanZero_WithNegative_AddsError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();

//        // Act
//        errors.IfHistoryLimitNotGreaterThanZero(-1);

//        // Assert
//        errors.Should().ContainSingle(e => e.Message == "The limit must be greater than zero.");
//    }

//    [Fact]
//    public void IfHistoryLimitNotGreaterThanZero_WithPositive_DoesNotAddError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();

//        // Act
//        errors.IfHistoryLimitNotGreaterThanZero(5);

//        // Assert
//        errors.Should().BeEmpty();
//    }

//    [Fact]
//    public void IfDateInFuture_WithFutureDate_AddsError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var future = DateTime.UtcNow.AddMinutes(10);

//        // Act
//        errors.IfDateInFuture(future);

//        // Assert
//        errors.Should().ContainSingle(e => e.Message == "The provided date cannot be in the future.");
//    }

//    [Fact]
//    public void IfDateInFuture_WithPastDate_DoesNotAddError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var past = DateTime.UtcNow.AddMinutes(-10);

//        // Act
//        errors.IfDateInFuture(past);

//        // Assert
//        errors.Should().BeEmpty();
//    }

//    [Fact]
//    public void IfDateRangeNotChronological_WithInvalidRange_AddsError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var start = new DateTime(2024, 1, 2);
//        var end = new DateTime(2024, 1, 1);

//        // Act
//        errors.IfDateRangeNotChronological(start, end);

//        // Assert
//        errors.Should().ContainSingle(e => e.Message.Contains("Invalid date range"));
//    }

//    [Fact]
//    public void IfDateRangeNotChronological_WithValidRange_DoesNotAddError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var start = new DateTime(2024, 1, 1);
//        var end = new DateTime(2024, 1, 2);

//        // Act
//        errors.IfDateRangeNotChronological(start, end);

//        // Assert
//        errors.Should().BeEmpty();
//    }

//    [Fact]
//    public void IfNoPropertyDetailsFound_WithInvalidProperty_AddsError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();

//        // Act
//        errors.IfNoPropertyDetailsFound("NotAProperty");

//        // Assert
//        errors.Should().ContainSingle(e => e.Message == "No public instance properties found in PropertyDetails.");
//    }

//    [Fact]
//    public void IfNoPropertyDetailsFound_WithValidProperty_DoesNotAddError()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var property = typeof(PropertyDetails)
//            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//            .First().Name;

//        // Act
//        errors.IfNoPropertyDetailsFound(property);

//        // Assert
//        errors.Should().BeEmpty();
//    }

//    [Fact]
//    public void CollectErrors_WithFailedResult_AddsApplicationErrors()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var appError = new global::Application.Extension.Error("fail");
//        var result = Result.Fail<string>(appError);

//        // Act
//        errors.CollectErrors(result);

//        // Assert
//        errors.Should().ContainSingle(e => e.Message == "fail");
//    }

//    [Fact]
//    public void CollectErrors_WithSuccessResult_DoesNotAddErrors()
//    {
//        // Arrange
//        var errors = new List<global::Application.Extension.Error>();
//        var result = Result.Ok("ok");

//        // Act
//        errors.CollectErrors(result);

//        // Assert
//        errors.Should().BeEmpty();
//    }
//}