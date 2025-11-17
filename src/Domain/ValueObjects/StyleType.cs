using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record StyleType : ValueObject<string>, ICreatable<StyleType, string?>
{
    public const int MaxLength = 30;

    private StyleType(string value) : base(value) { }

    public static Result<StyleType> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, StyleType>(value)
            .Congregate(pipeline => pipeline
                .IfLengthTooLong<DomainLayer, StyleType>(value, MaxLength)
                .IfStyleTypeNotInclude<DomainLayer>(value))
            .ExecuteIfNoErrors<StyleType>(() => new StyleType(value!))
            .MapResult<StyleType>();

        return result;
    }
}

internal static class StyleTypeErrorsExtensions
{
    internal static WorkflowPipeline IfStyleTypeNotInclude<TLayer>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (!Enum.TryParse<StyleTypeEnum>(value, true, out var _))
        {
            pipeline.Errors.Add(ErrorFactories.OptionNotAllowed<StyleTypeEnum, TLayer>(value, typeof(StyleTypeEnum)));
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
