using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;

public class ExampleLinksControllerTestsBase : ControllerTestsBase
{
    // URL constants
    protected const string CorrectUrl = "http://example.com/image.jpg";
    protected const string AlreadyExistUrl = "http://example.com/existing-image.jpg";
    protected const string IncorrectUrl = "not a valid url";

    // Style name constants
    protected const string CorrectStyleName = "ModernArt";
    protected const string IncorrectStyleName = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor";
    protected const string AlreadyExistStyleName = "AlreadyExist";
    protected const string NonExistStyleName = "NonExistentStyle";

    // Version constants
    protected const string CorrectVersion = "1";
    protected const string CorrectNijiVersion = "niji 1";
    protected const string IncorrectVersion = "invalid-version";
    protected const string NonExistVersion = "99.0";

    // Error message
    protected const string ErrorMessageInvalidLinkFormat = "Invalid link format";
    protected const string ErrorMessageStyleNameTooLong = "Style name exceeds maximum length";
    protected const string ErrorMessageInvalidVersionFormat = "Invalid version format";
    protected const string ErrorMessageAllFieldsRequired = "All fields are required";
    protected const string ErrorMessageLinkAlreadyExists = "Link already exists";
    protected const string ErrorMessageStyleNotFound = $"Style '{NonExistStyleName}' already exist";
    protected const string ErrorMessageVersionNotFound = $"Version '{NonExistVersion}' not found";
    protected const string ErrorMessageStyleAndVersionNotFound = "Style and version not found";
    protected const string ErrorMessageInvalidInputData = "Invalid input data";
    protected const string ErrorCanceledOperation = "The operation was canceled.";

    // ID constant
    protected static readonly string CorrectId = Guid.NewGuid().ToString();


    // Standard valid request
    protected static readonly AddExampleLinkRequest requestOk = new
    (
        CorrectUrl,
        CorrectStyleName,
        CorrectVersion
    );

    // Request with invalid URL
    protected static readonly AddExampleLinkRequest requestInvalidUrl = new
    (
        IncorrectUrl,
        CorrectStyleName,
        CorrectVersion
    );

    // Request with invalid style name (too long)
    protected static readonly AddExampleLinkRequest requestInvalidStyleName = new
    (
        CorrectUrl,
        IncorrectStyleName,
        CorrectVersion
    );

    // Request with invalid version
    protected static readonly AddExampleLinkRequest requestInvalidVersion = new
    (
        CorrectUrl,
        CorrectStyleName,
        IncorrectVersion
    );

    // Request with all empty fields
    protected static readonly AddExampleLinkRequest requestAllEmpty = new
    (
        string.Empty,
        string.Empty,
        string.Empty
    );

    // Request for existing link scenario
    protected static readonly AddExampleLinkRequest requestExistingLink = new
    (
        AlreadyExistUrl,
        CorrectStyleName,
        CorrectVersion
    );

    // Request with non-existent style
    protected static readonly AddExampleLinkRequest requestNonExistentStyle = new
    (
        CorrectUrl,
        NonExistStyleName,
        CorrectVersion
    );

    // Request with non-existent version
    protected static readonly AddExampleLinkRequest requestNonExistentVersion = new
    (
        CorrectUrl,
        CorrectStyleName,
        NonExistVersion
    );

    // Request with both non-existent style and version
    protected static readonly AddExampleLinkRequest requestNonExistentBoth = new
    (
        CorrectUrl,
        NonExistStyleName,
        NonExistVersion
    );

    // Standard success result
    protected static readonly Result<string> resultOk = Result.Ok(CorrectId);

    // Failure results for AddExampleLink scenarios
    protected static readonly Result<string> failureInvalidLinkFormat = 
        CreateFailureResult<string, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            ErrorMessageInvalidLinkFormat
        );

    protected static readonly Result<string> failureStyleNameTooLong = 
        CreateFailureResult<string, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            ErrorMessageStyleNameTooLong
        );

    protected static readonly Result<string> failureInvalidVersionFormat = 
        CreateFailureResult<string, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            ErrorMessageInvalidVersionFormat
        );

    protected static readonly Result<string> failureAllFieldsRequired = 
        CreateFailureResult<string, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            ErrorMessageAllFieldsRequired
        );

    protected static readonly Result<string> failureInvalidInputData = 
        CreateFailureResult<string, DomainLayer>
        (
            StatusCodes.Status400BadRequest,
            ErrorMessageInvalidInputData
        );

    protected static readonly Result<string> failureLinkAlreadyExists = 
        CreateFailureResult<string, ApplicationLayer>
        (
            StatusCodes.Status409Conflict,
            ErrorMessageLinkAlreadyExists
        );

    protected static readonly Result<string> failureStyleNotFound = 
        CreateFailureResult<string, ApplicationLayer>
        (
            StatusCodes.Status409Conflict,
            ErrorMessageStyleNotFound
        );

    protected static readonly Result<string> failureVersionNotFound = 
        CreateFailureResult<string, ApplicationLayer>
        (
            StatusCodes.Status404NotFound,
            ErrorMessageVersionNotFound
        );

    protected static readonly Result<string> failureStyleAndVersionNotFound = 
        CreateFailureResult<string, ApplicationLayer>
        (
            StatusCodes.Status404NotFound,
            ErrorMessageStyleAndVersionNotFound
        );

    // Factory method for controller
    protected static ExampleLinksController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new ExampleLinksController(sender);
    }

    protected static Mock<ISender> CreateSenderMock() => new();
}