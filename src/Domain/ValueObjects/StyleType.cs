using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.ValueObjects;

public record StyleType : ValueObject<string>, ICreatable<StyleType, string>
{
    public const int MaxLength = 30;

    private StyleType(string value) : base(value) { }

    public static Result<StyleType> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, StyleType>(value)
            .IfLengthTooLong<DomainLayer, StyleType>(value, MaxLength)
            .IfStyleTypeNotInclude<DomainLayer>(value);

        if (errors.Count != 0)
            return Result.Fail<StyleType>(errors);

        return Result.Ok(new StyleType(value));
    }
}
internal static class StyleNameErrorsExtensions
{
    internal static List<Error> IfStyleTypeNotInclude<TLayer>(this List<Error> domainErrors, string value)
        where TLayer : ILayer
    {
        if (!Enum.TryParse<StyleTypeEnum>(value, true, out var _))
        {
            domainErrors.Add(new Error<TLayer>
            (
                $"Invalid style type: {value}. Expected values are: {string.Join(", ", Enum.GetNames<StyleTypeEnum>())}"
            ));
        }

        return domainErrors;
    }
}

public enum StyleTypeEnum
{
    Custom,
    StyleReferences,
    Personalization
}
