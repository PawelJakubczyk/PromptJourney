using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;

namespace Unit.Utilities.Tests.Tests.Utilities;

public sealed class TestsErrors
{
    public static Error notSupportedEx => ErrorBuilder.New()
        .WithLayer<PresentationLayer>()
        .WithMessage(notSupportedEx.Message)
        .WithErrorCode(StatusCodes.Status400BadRequest)
        .Build();

    public static Error unauthorizedEx => ErrorBuilder.New()
        .WithLayer<PresentationLayer>()
        .WithMessage(unauthorizedEx.Message)
        .WithErrorCode(StatusCodes.Status401Unauthorized)
        .Build();

    public static Error fileNotFoundEx => ErrorBuilder.New()
        .WithLayer<InfrastructureLayer>()
        .WithMessage(fileNotFoundEx.Message)
        .WithErrorCode(StatusCodes.Status404NotFound)
        .Build();

    public static Error dirNotFoundEx => ErrorBuilder.New()
        .WithLayer<InfrastructureLayer>()
        .WithMessage(dirNotFoundEx.Message)
        .WithErrorCode(StatusCodes.Status404NotFound)
        .Build();
}