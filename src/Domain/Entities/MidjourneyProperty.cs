using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public sealed class MidjourneyProperty : IEntity
{
    // Columns
    public PropertyName PropertyName { get; private set; }
    public ModelVersion Version { get; private set; }
    public ParamsCollection Parameters { get; private set; }
    public DefaultValue DefaultValue { get; private set; }
    public MinValue MinValue { get; private set; }
    public MaxValue MaxValue { get; private set; }
    public Description Description { get; private set; }
    // Navigation
    public MidjourneyVersion MidjourneyVersion { get; private set; } = null!;
    // Constructors
    private MidjourneyProperty
    (
        PropertyName propertyName,
        ModelVersion version,
        ParamsCollection parameters,
        DefaultValue defaultValue,
        MinValue minValue,
        MaxValue maxValue,
        Description description
    )
    {
        PropertyName = propertyName!;
        Version = version!;
        Parameters = parameters;
        DefaultValue = defaultValue;
        MinValue = minValue;
        MaxValue = maxValue;
        Description = description;
    }

    public static Result<MidjourneyProperty> Create
    (
        Result<PropertyName> propertyNameResult,
        Result<ModelVersion> versionResult,
        Result<ParamsCollection> paramsCollectionResult,
        Result<DefaultValue> defaultValueResult,
        Result<MinValue> minValueResult,
        Result<MaxValue> maxValueResult,
        Result<Description> descriptionResult
    )
    {
        var result = WorkflowPipeline
        .Empty()
        .CongregateErrors(
            pipeline => pipeline.CollectErrors(propertyNameResult),
            pipeline => pipeline.CollectErrors(versionResult),
            pipeline => pipeline.CollectErrors(paramsCollectionResult),
            pipeline => pipeline.CollectErrors(defaultValueResult),
            pipeline => pipeline.CollectErrors(minValueResult),
            pipeline => pipeline.CollectErrors(maxValueResult),
            pipeline => pipeline.CollectErrors(descriptionResult))
        .ExecuteIfNoErrors(() =>
        {
            var versionBase = new MidjourneyProperty
            (
                propertyNameResult.Value,
                versionResult.Value,
                paramsCollectionResult.Value,
                defaultValueResult.Value,
                minValueResult.Value,
                maxValueResult.Value,
                descriptionResult.Value
            );

            return Result.Ok(versionBase);
        })
        .MapResult<MidjourneyProperty>();

        return result;
    }

    public Result<ParamsCollection> UpdateParamsCollection(Result<ParamsCollection> paramsCollection) =>
        UpdateValue(paramsCollection);

    public Result<DefaultValue> UpdateDefaultValue(Result<DefaultValue> defaultValue) =>
        UpdateValue(defaultValue);

    public Result<MinValue> UpdateMinValue(Result<MinValue> minValue) =>
        UpdateValue(minValue);

    public Result<MaxValue> UpdateMaxValue(Result<MaxValue> maxValue) =>
        UpdateValue(maxValue);

    public Result<Description> UpdateDescription(Result<Description> description) =>
        UpdateValue(description);

    private Result<TValue> UpdateValue<TValue>(Result<TValue> valueResult)
    {
        var result = WorkflowPipeline
            .Empty()
            .CollectErrors(valueResult)
            .ExecuteIfNoErrors<TValue>(() => valueResult.Value)
            .MapResult<TValue>();

        return result;
    }
}


