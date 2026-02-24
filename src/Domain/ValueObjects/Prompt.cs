using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Constants;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record Prompt : ValueObject<string>, ICreatable<Prompt, string?>
{
    public const int MaxLength = 1000;

    private Prompt(string value) : base(value) { }

    public static Result<Prompt> Create(string? value)
    {
        value ??= string.Empty;

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<DomainLayer, Prompt>(value, MaxLength)
            .ExecuteIfNoErrors<Prompt>(() => new Prompt(value))
            .MapResult<Prompt>();

        return result;
    }
}
