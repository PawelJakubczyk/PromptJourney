using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record DefaultValue : ValueObject<string?>, ICreatable<DefaultValue, string?>
{
    public const int MaxLength = 50;

    private DefaultValue(string? value) : base(value) { }

    public static Result<DefaultValue> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = null;

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<DomainLayer, DefaultValue>(value, MaxLength)
            .IfContainsSuspiciousContent<DomainLayer, DefaultValue>(value)
            .ExecuteIfNoErrors<DefaultValue>(() => new DefaultValue(value))
            .MapResult<DefaultValue>();

        return result;
    }
}
