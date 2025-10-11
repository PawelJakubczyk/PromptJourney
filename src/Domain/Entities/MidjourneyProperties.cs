using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;
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
    protected MidjourneyProperties()
    {
        // Parameterless constructor for EF Core
    }

    protected MidjourneyProperties
    (
        PropertyName propertyName,
        ModelVersion version,
        List<Param>? parameters = null,
        DefaultValue? defaultValue = null,
        MinValue? minValue = null,
        MaxValue? maxValue = null,
        Description? description = null
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
        List<Result<Param>>? paramResultsList = null,
        Result<DefaultValue?>? defaultValueResult = null,
        Result<MinValue?>? minValueResult = null,
        Result<MaxValue?>? maxValueResult = null,
        Result<Description?>? descriptionResult = null
    )
    {
        var result = WorkflowPipeline
        .Empty()
        .Validate(pipeline => pipeline
            .CollectErrors<PropertyName>(propertyNameResult)
            .CollectErrors<ModelVersion>(versionResult)
            .CollectErrors<Param>(paramResultsList)
            .CollectErrors<DefaultValue>(defaultValueResult)
            .CollectErrors<MinValue>(minValueResult)
            .CollectErrors<MaxValue>(maxValueResult)
            .CollectErrors<Description>(descriptionResult)
            .IfListIsEmpty<DomainLayer, Param>(paramResultsList?.ToValueList())
            .IfListHasDuplicates<DomainLayer, Param>(paramResultsList?.ToValueList()))
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
