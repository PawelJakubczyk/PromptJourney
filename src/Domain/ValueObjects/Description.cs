using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Description : ValueObject<string?>, ICreatable<Description, string?>
{
    public const int MaxLength = 500;

    private Description(string? value) : base(value) { }

    public static Result<Description> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfWhitespace<DomainLayer, Description>(value)
            .IfLengthTooLong<DomainLayer, Description>(value, MaxLength)
            .ExecuteIfNoErrors<Description>(() => new Description(value))
            .MapResult<Description>();

        return result;
    }
}
