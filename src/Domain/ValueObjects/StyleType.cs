using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record StyleType : ValueObject<string>, ICreatable<StyleType, string>
{
    public const int MaxLength = 30;

    private StyleType(string value) : base(value) { }

    public static Result<StyleType> Create(string value)
    {
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .IfNullOrWhitespace<DomainLayer, StyleType>(value)
                .IfLengthTooLong<DomainLayer, StyleType>(value, MaxLength)
                .IfStyleTypeNotInclude<DomainLayer>(value))
            .ExecuteIfNoErrors<StyleType>(() => new StyleType(value))
            .MapResult(s => s);

        return result;
    }
}

internal static class StyleTypeErrorsExtensions
{
    internal static WorkflowPipeline IfStyleTypeNotInclude<TLayer>(this WorkflowPipeline pipeline, string value)
        where TLayer : ILayer
    {
        if (!Enum.TryParse<StyleTypeEnum>(value, true, out var _))
        {
            pipeline.Errors.Add(new Error<TLayer>
            (
                $"Invalid style type: {value}. Expected values are: {string.Join(", ", Enum.GetNames<StyleTypeEnum>())}",
                StatusCodes.Status400BadRequest
            ));
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
