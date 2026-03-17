using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record StyleName : ValueObject<string>, ICreatable<StyleName, string>
{
    public const int MaxLength = 150;
    public override bool IsNone => false;

    private StyleName(string value) : base(value) { }

    public static Result<StyleName> Create(string value)
    {
        value = value?.Trim().ToLower();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<StyleName>(value)
            .IfLengthTooLong<StyleName>(value, MaxLength)
            .IfContainsSuspiciousContent<StyleName>(value)
            .ExecuteIfNoErrors<StyleName>(() => new StyleName(value!))
            .MapResult<StyleName>();

        return result;
    }
}
