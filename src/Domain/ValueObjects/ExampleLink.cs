using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class ExampleLink
{
    public const int MaxLength = 200;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private readonly bool isFailed;

    private ExampleLink(string value)
    {
        Value = value;
    }

    public static Result<ExampleLink> Create(string value)
    {
        _errors.Clear();

        ValidateLinkNotEmpty(value);
        ValidateLinkLength(value);
        ValidateLinkFormat(value);

        if (_errors.Any())
            return Result.Fail<ExampleLink>(_errors);

        return Result.Ok(new ExampleLink(value));
    }

    private static void ValidateLinkNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(ExampleLinkNullOrEmptyError);
        }
    }

    private static void ValidateLinkLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(ExampleLinkTooLongError.WithDetail($"link length: {value.Length}"));
        }
    }

    private static void ValidateLinkFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (!IsValidUrl(value))
        {
            _errors.Add(new DomainError($"Invalid URL format: {value}"));
        }
    }

    private static bool IsValidUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    public override string ToString() => Value;
}