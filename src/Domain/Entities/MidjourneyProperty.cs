using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyProperty : IEntity
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
    public MidjourneyVersion MidjourneyVersion { get; private set; }
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
        var paramsCollectionResultNonNull = paramsCollectionResult ?? Result<ParamsCollection>.Ok(ParamsCollection.None);
        var defaultValueResultNonNull = defaultValueResult ?? Result<DefaultValue>.Ok(DefaultValue.None);
        var minValueResultNonNull = minValueResult ?? Result<MinValue>.Ok(MinValue.None);
        var maxValueResultNonNull = maxValueResult ?? Result<MaxValue>.Ok(MaxValue.None);
        var descriptionResultNonNull = descriptionResult ?? Result<Description>.Ok(Description.None);

        var result = WorkflowPipeline
        .Empty()
        .CongregateErrors(
            pipeline => pipeline.CollectErrors(propertyNameResult),
            pipeline => pipeline.CollectErrors(versionResult),
            pipeline => pipeline.CollectErrors(paramsCollectionResultNonNull),
            pipeline => pipeline.CollectErrors(defaultValueResultNonNull),
            pipeline => pipeline.CollectErrors(minValueResultNonNull),
            pipeline => pipeline.CollectErrors(maxValueResultNonNull),
            pipeline => pipeline.CollectErrors(descriptionResultNonNull))
        .ExecuteIfNoErrors(() =>
        {
            var versionBase = new MidjourneyProperty
            (
                propertyNameResult.Value,
                versionResult.Value,
                paramsCollectionResultNonNull.Value,
                defaultValueResultNonNull.Value,
                minValueResultNonNull.Value,
                maxValueResultNonNull.Value,
                descriptionResultNonNull.Value
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


