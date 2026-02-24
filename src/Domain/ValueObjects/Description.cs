using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Description : ValueObject<string?>, ICreatable<Description?, string?>
{
    public const int MaxLength = 500;

    private Description(string? value) : base(value) { }

    public static Result<Description?> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = null;

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<DomainLayer, Description>(value, MaxLength)
            .IfContainsSuspiciousContent<DomainLayer, Description>(value)
            .ExecuteIfNoErrors<Description>(() => new Description(value))
            .MapResult<Description?>();

        return result;
    }
}