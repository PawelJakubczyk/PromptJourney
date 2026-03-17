using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record PropertyName : ValueObject<string>, ICreatable<PropertyName, string>
{
    public const int MaxLength = 25;
    public override bool IsNone => false;

    private PropertyName(string value) : base(value) { }

    public static Result<PropertyName> Create(string value)
    {
        value = value?.Trim().ToLower();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<PropertyName>(value)
            .IfLengthTooLong<PropertyName>(value, MaxLength)
            .IfContainsSuspiciousContent<PropertyName>(value)
            .ExecuteIfNoErrors<PropertyName>(() => new PropertyName(value!))
            .MapResult<PropertyName>();

        return result;
    }
}
