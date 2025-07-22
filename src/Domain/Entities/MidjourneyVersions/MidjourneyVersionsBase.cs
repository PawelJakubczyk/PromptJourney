using Domain.Entities.MidjourneyVersions.Exceptions;
using Domain.ErrorMassages;
using Domain.Exceptions;
using FluentResults;

namespace Domain.Entities.MidjourneyVersions;

public class MidjourneyVersionsBase
{
    // Columns
    public string PropertyName { get; set; }
    public string Version { get; set; }
    public string[]? Parameters { get; set; }
    public string? DefaultValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public string? Description { get; set; }

    // Navigation
    public MidjourneyVersionsMaster VersionMaster { get; set; }

    // Errors
    private static List<MidjourneyEntitiesException> _errors = [];

    // Constructor
    protected MidjourneyVersionsBase()
    {
    }

    protected MidjourneyVersionsBase
    (
        string propertyName, 
        string version,
        string[]? parameters = null, 
        string? defaultValue = null, 
        string? minValue = null, 
        string? maxValue = null, 
        string? description = null
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

    public static Result<MidjourneyVersionsBase> Create
    (
        string propertyName, 
        string version,
        string[]? parameters = null, 
        string? defaultValue = null, 
        string? minValue = null, 
        string? maxValue = null, 
        string? description = null
    )
    {
        ValidatePropertyName(propertyName);
        ValidateVersion(version);
        ValidateParameters(parameters);
        ValidateDefaultValue(defaultValue);
        ValidateMinValue(minValue);
        ValidateMaxValue(maxValue);
        ValidateDescription(description);

        if (_errors.Count > 0)
        {
            return Result.Fail<MidjourneyVersionsBase>(_errors.Select(e => e.Message));
        }

        var versionBase = new MidjourneyVersionsBase
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

    private static void ValidatePropertyName(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            _errors.Add(new VersionValidationException($"PropertyName: {propertyName}", ErrorMessages.PropertyNameNullOrEmpty));
        else if (propertyName.Length > 25)
            _errors.Add(new VersionValidationException($"PropertyName: {propertyName}", ErrorMessages.PropertyNameTooLong));
    }

    private static void ValidateVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            _errors.Add(new VersionValidationException("Version", ErrorMessages.VersionNullOrEmpty));
        else if (version.Length > 10)
            _errors.Add(new VersionValidationException("Version", ErrorMessages.VersionTooLong));
    }

    private static void ValidateParameters(string[]? parameters)
    {
        if (parameters != null && parameters.Length == 0)
            _errors.Add(new VersionValidationException("Parameters", ErrorMessages.ParametersEmpty));
        else if (parameters != null && parameters.Length > 10)
            _errors.Add(new VersionValidationException("Parameters", ErrorMessages.ParametersTooMany));

        foreach (var parameter in parameters ?? [])
        {
            if (string.IsNullOrEmpty(parameter))
                _errors.Add(new VersionValidationException($"Parameter: {parameter}", ErrorMessages.ParameterNullOrEmpty));
            else if (parameter.Length > 100)
                _errors.Add(new VersionValidationException($"Parameter: {parameter}", ErrorMessages.ParameterTooLong));
        }
    }

    private static void ValidateDefaultValue(string? defaultValue)
    {
        if (defaultValue != null && defaultValue.Length == 0)
            _errors.Add(new VersionValidationException("DefaultValue", ErrorMessages.DefaultValueEmpty));
        else if (defaultValue != null && defaultValue.Length > 50)
            _errors.Add(new VersionValidationException("DefaultValue", ErrorMessages.DefaultValueTooLong));
    }

    private static void ValidateMinValue(string? minValue)
    {
        if (minValue != null && minValue.Length == 0)
            _errors.Add(new VersionValidationException("MinValue", ErrorMessages.MinValueEmpty));
        else if (minValue != null && minValue.Length > 50)
            _errors.Add(new VersionValidationException("MinValue", ErrorMessages.MinValueTooLong));
    }

    private static void ValidateMaxValue(string? maxValue)
    {
        if (maxValue != null && maxValue.Length == 0)
            _errors.Add(new VersionValidationException("MaxValue", ErrorMessages.MaxValueEmpty));
        else if (maxValue != null && maxValue.Length > 50)
            _errors.Add(new VersionValidationException("MaxValue", ErrorMessages.MaxValueTooLong));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length == 0)
            _errors.Add(new VersionValidationException("Description", ErrorMessages.DescriptionEmpty));
        else if (description != null && description.Length > 500)
            _errors.Add(new VersionValidationException("Description", ErrorMessages.DescriptionTooLong));
    }
}
