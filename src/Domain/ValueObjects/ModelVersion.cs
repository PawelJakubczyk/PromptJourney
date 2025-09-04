using Domain.Errors;
using FluentResults;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class ModelVersion
{
    public const int MaxLength = 10;
    public string Value { get; }

    private ModelVersion(string value)
    {
        Value = value;
    }

    public static Result<ModelVersion> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<ModelVersion>(value)
            .IfLengthTooLong<ModelVersion>(value, MaxLength)
            .IfVersionFormatInvalid(value);

        if (errors.Count != 0)
            return Result.Fail<ModelVersion>(errors);

        return Result.Ok(new ModelVersion(value));
    }

    public override string ToString() => Value;
};

public static class ModelVersionErrorsExtensions
{
    internal static List<DomainError> IfVersionFormatInvalid(this List<DomainError> domainErrors, string value)
    {
        bool isValidNumeric = _validNumericRegex.IsMatch(value);
        bool isValidNiji = _validNijiRegex.IsMatch(value);

        if (!isValidNumeric && !isValidNiji)
        {
            domainErrors.Add(new DomainError
            (
                $"Invalid version format: {value}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')"
            ));
        }

        return domainErrors;
    }

    private static readonly Regex _validNumericRegex = new(@"^niji [4-6]$", RegexOptions.Compiled);
    private static readonly Regex _validNijiRegex = new(@"^[1-9](\.[0-9])?$", RegexOptions.Compiled);
}
