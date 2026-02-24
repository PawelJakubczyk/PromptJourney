using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyProperties : IEntity
{
    // Columns
    public PropertyName PropertyName { get; set; }
    public ModelVersion Version { get; set; }
    public List<Param>? Parameters { get; set; }
    public DefaultValue? DefaultValue { get; set; }
    public MinValue? MinValue { get; set; }
    public MaxValue? MaxValue { get; set; }
    public Description? Description { get; set; }

    // Navigation
    public MidjourneyVersion MidjourneyVersion { get; set; }

    // Constructors
    private MidjourneyProperties
    (
        PropertyName propertyName,
        ModelVersion version,
        List<Param>? parameters,
        DefaultValue? defaultValue,
        MinValue? minValue,
        MaxValue? maxValue,
        Description? description
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

    public static Result<MidjourneyProperties> Create
    (
        Result<PropertyName> propertyNameResult,
        Result<ModelVersion> versionResult,
        List<Result<Param>> paramResultsList = null,
        Result<DefaultValue?>? defaultValueResult = null,
        Result<MinValue?>? minValueResult = null,
        Result<MaxValue?>? maxValueResult = null,
        Result<Description?>? descriptionResult = null
    )
    {
        if (paramResultsList != null && paramResultsList.Count == 0)
        {
            paramResultsList = null;
        }

        var result = WorkflowPipeline
        .Empty()
        .CongregateErrors(
            pipeline => pipeline.CollectErrors(propertyNameResult),
            pipeline => pipeline.CollectErrors(versionResult),
            pipeline => pipeline.CollectErrors(paramResultsList?.ToArray()),
            pipeline => pipeline.CollectErrors(defaultValueResult),
            pipeline => pipeline.CollectErrors(minValueResult),
            pipeline => pipeline.CollectErrors(maxValueResult),
            pipeline => pipeline.CollectErrors(descriptionResult),
            pipeline => pipeline.IfListHasDuplicates<DomainLayer, Param>(paramResultsList?.ToValueList()))
        .ExecuteIfNoErrors(() =>
        {
            var versionBase = new MidjourneyProperties
            (
                propertyNameResult.Value,
                versionResult.Value,
                paramResultsList.ToValueList(),
                defaultValueResult?.Value,
                minValueResult?.Value,
                maxValueResult?.Value,
                descriptionResult?.Value
            );

            return Result.Ok(versionBase);
        })
        .MapResult<MidjourneyProperties>();

        return result;
    }

    public static Result<Description> EditDescription(Result<Description?> description)
    {
        var result = WorkflowPipeline
            .Empty()
            .CollectErrors(description)
            .ExecuteIfNoErrors<Description>(() => description.Value)
            .MapResult<Description>();

        return result;
    }
}
