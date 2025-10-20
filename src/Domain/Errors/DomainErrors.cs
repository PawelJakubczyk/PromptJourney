using Domain.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;

namespace Domain.Errors;

public static class DomainErrors
{
    public static Error ExamleLinkNotFound(Guid id) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage($"Example link with ID {id} not found")
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error HistoryNotFoundError(Guid historyId) => 
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage($"History record with ID {historyId} not found")
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();


    public static Error VersionNotFound(ModelVersion modelVersion) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage($"Version '{modelVersion.Value}' not found")
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error NoVersionFound() =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage("No version found")
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error StyleNotFound(StyleName style) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage($"Style with name '{style.Value}' not found")
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error PropertyNotFound(PropertyName propertyName) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage($"Property '{propertyName.Value}' not found")
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error ParameterInvalid(Param parameter) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage($"Parameter '{parameter.Value}' is invalid")
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

}
