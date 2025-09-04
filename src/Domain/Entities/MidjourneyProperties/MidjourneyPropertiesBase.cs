using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;


namespace Domain.Entities.MidjourneyProperties;

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
    public MidjourneyVersions.MidjourneyVersion VersionMaster { get; set; }

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
        PropertyName propertyName,
        ModelVersion version,
        List<Param>? parameters = null,
        DefaultValue? defaultValue = null,
        MinValue? minValue = null,
        MaxValue? maxValue = null,
        Description? description = null
    )
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors<PropertyName>(propertyName)
            .CollectErrors<ModelVersion>(version)
            .CollectErrors<List<Param>?>(parameters)
            .CollectErrors<DefaultValue?>(defaultValue)
            .CollectErrors<MinValue?>(minValue)
            .CollectErrors<MaxValue?>(maxValue)
            .CollectErrors<Description?>(description);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyPropertiesBase>(errors);

        var versionBase = new MidjourneyPropertiesBase
        (
            propertyName, 
            version, 
            parameters, 
            defaultValue, 
            minValue, 
            maxValue, 
            description
        );

        return Result.Ok(versionBase);
    }
}
