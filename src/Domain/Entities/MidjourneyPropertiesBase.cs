using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;


namespace Domain.Entities;

public class MidjourneyPropertiesBase
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
    public MidjourneyVersion VersionMaster { get; set; }

    // Constructors
    protected MidjourneyPropertiesBase()
    {
        // Parameterless constructor for EF Core
    }

    protected MidjourneyPropertiesBase
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

    public static Result<MidjourneyPropertiesBase> Create
    (
        Result<PropertyName> propertyNameResult,
        Result<ModelVersion> versionResult,
        List<Result<Param>>? parametersResultsList = null,
        Result<DefaultValue?>? defaultValueResult = null,
        Result<MinValue?>? minValueResult = null,
        Result<MaxValue?>? maxValueResult = null,
        Result<Description?>? descriptionResult = null
    )
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors(propertyNameResult)
            .CollectErrors(versionResult)
            .CollectErrors(defaultValueResult)
            .CollectErrors(minValueResult)
            .CollectErrors(maxValueResult)
            .CollectErrors(descriptionResult);

        List<Param> parameterslList = [];

        foreach (var parametersResult in parametersResultsList ?? [])
        {
            errors.CollectErrors(parametersResult);
            if (parametersResult.IsSuccess)
                parameterslList.Add(parametersResult.Value);
        };

        if (errors.Count != 0)
            return Result.Fail<MidjourneyPropertiesBase>(errors);

        var versionBase = new MidjourneyPropertiesBase
        (
            propertyNameResult.Value, 
            versionResult.Value,
            parameterslList, 
            defaultValueResult?.Value, 
            minValueResult?.Value, 
            maxValueResult?.Value, 
            descriptionResult?.Value
        );

        return Result.Ok(versionBase);
    }
}
