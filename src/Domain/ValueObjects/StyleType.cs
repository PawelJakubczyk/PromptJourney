using Domain.Abstractions;
using Domain.Errors;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record StyleType : ValueObject<string>, ICreatable<StyleType, string>
{
    public const int MaxLength = 16;
    public override bool IsNone => false;

    private StyleType(string value) : base(value) { }

    public static Result<StyleType> Create(string value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<StyleType>(value)
            .IfStyleTypeNotInclude(value)
            .ExecuteIfNoErrors<StyleType>(() => new StyleType(value!))
            .MapResult<StyleType>();

        return result;
    }
}

internal static class StyleTypeErrorsExtensions
{
    internal static WorkflowPipeline IfStyleTypeNotInclude(
        this WorkflowPipeline pipeline,
        string? value)
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (!Enum.TryParse<StyleTypeEnum>(value, true, out var _))
        {
            pipeline.Errors.Add(DomainErrors.InvalidStyleTypeNotAllowed(value, typeof(StyleTypeEnum)));
        }

        return pipeline;
    }
}

public enum StyleTypeEnum
{
    Custom,
    StyleReferences,
    Personalization
}
