using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class Prompt
{
    public const int MaxLength = 1000;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private Prompt(string value)
    {
        Value = value;
    }

    public static Result<Prompt> Create(string value)
    {
        _errors.Clear();

        ValidatePromptNotEmpty(value);
        ValidatePromptLength(value);

        if (_errors.Count != 0)
            return Result.Fail<Prompt>(_errors);

        return Result.Ok(new Prompt(value));
    }

    private static void ValidatePromptNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(PromptNullOrEmptyError);
        }
    }

    private static void ValidatePromptLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(PromptToLongError.WithDetail($"prompt length: {value.Length}"));
        }
    }

    public override string ToString() => Value;
}