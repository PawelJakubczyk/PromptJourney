using Domain.Abstractions;
using Domain.Errors;
using FluentResults;
using System.Text.RegularExpressions;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.ValueObjects;

public record ModelVersion : ValueObject<string>, ICreatable<ModelVersion, string>
{
    public const int MaxLength = 10;
    private ModelVersion(string value) : base(value) { }

    public static Result<ModelVersion> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, ModelVersion>(value)
            .IfLengthTooLong<DomainLayer, ModelVersion>(value, MaxLength)
            .IfVersionFormatInvalid<DomainLayer>(value);

        if (errors.Count != 0)
            return Result.Fail<ModelVersion>(errors);

        return Result.Ok(new ModelVersion(value));
    }
}

internal static class ModelVersionErrorsExtensions
{
    internal static List<Error> IfVersionFormatInvalid<TLayer>(this List<Error> Errors, string value)
        where TLayer : ILayer
    {
        bool isValidNumeric = _validNumericRegex.IsMatch(value);
        bool isValidNiji = _validNijiRegex.IsMatch(value);

        if (!isValidNumeric && !isValidNiji)
        {
            Errors.Add(new Error<TLayer>
            (
                $"Invalid version format: {value}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')"
            ));
        }

        return Errors;
    }

    private static readonly Regex _validNumericRegex =
        new(@"^[1-9][0-9]*(\.[0-9])?$", RegexOptions.Compiled);

    private static readonly Regex _validNijiRegex =
        new(@"^niji [1-9][0-9]*$", RegexOptions.Compiled);
}
