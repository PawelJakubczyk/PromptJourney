using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class ModelVersion
{
    public const int MaxLength = 10;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private ModelVersion(string value)
    {
        Value = value;
    }

    public static Result<ModelVersion> Create(string value)
    {
        ValidateVersionNotEmpty(value);
        ValidateVersionLength(value);
        ValidateVersionFormat(value);

        if (_errors.Count != 0)
            return Result.Fail<ModelVersion>(_errors);

        return Result.Ok(new ModelVersion(value));
    }

    private static void ValidateVersionNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(VersionNullOrEmptyError);
        }
    }

    private static void ValidateVersionLength(string value)
    {
        if (value.Length > MaxLength)
        {
            _errors.Add(VersionToLongError.WithDetail($"version length: {value.Length}"));
        }
    }

    private static void ValidateVersionFormat(string value)
    {
        var isValidNumeric = ValidNumericRegex().IsMatch(value);
        var isValidNiji = ValidNijiRegex().IsMatch(value);

        if (!isValidNumeric && !isValidNiji)
        {
            _errors.Add(new DomainError($"Invalid version format: {value}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')"));
        }
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^\d+(\.\d+)?$")]
    private static partial Regex ValidNumericRegex();

    [GeneratedRegex(@"^niji \d+$")]
    private static partial Regex ValidNijiRegex();
};

