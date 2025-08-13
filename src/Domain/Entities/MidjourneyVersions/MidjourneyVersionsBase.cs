using FluentResults;
using static Domain.Errors.DomainErrorMessages;

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
    private static List<DomainError> _errors = [];

    // Constructor
    protected MidjourneyVersionsBase()
    {
        // Parameterless constructor for EF Core
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
        _errors.Clear();
        
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
            _errors.Add(PropertyNameNullOrEmptyError);
        else if (propertyName.Length > 25)
            _errors.Add(PropertyNameTooLongError.WithDetail($"property name: '{propertyName}' (length: {propertyName.Length})"));
    }

    private static void ValidateVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            _errors.Add(VersionNullOrEmptyError);
        else if (version.Length > 10)
            _errors.Add(VersionToLongError.WithDetail($"version: '{version}' (length: {version.Length})"));
    }

    private static void ValidateParameters(string[]? parameters)
    {
        if (parameters != null && parameters.Length == 0)
            _errors.Add(ParametersEmptyError);
        else if (parameters != null && parameters.Length > 10)
            _errors.Add(ParametersTooManyError.WithDetail($"parameter count: {parameters.Length}"));

        foreach (var parameter in parameters ?? [])
        {
            if (string.IsNullOrEmpty(parameter))
                _errors.Add(ParameterNullOrEmptyError);
            else if (parameter.Length > 100)
                _errors.Add(ParameterTooLongError.WithDetail($"parameter: '{parameter}' (length: {parameter.Length})"));
        }
    }

    private static void ValidateDefaultValue(string? defaultValue)
    {
        if (defaultValue != null && defaultValue.Length == 0)
            _errors.Add(DefaultValueEmptyError);
        else if (defaultValue != null && defaultValue.Length > 50)
            _errors.Add(DefaultValueTooLongError.WithDetail($"default value length: {defaultValue.Length}"));
    }

    private static void ValidateMinValue(string? minValue)
    {
        if (minValue != null && minValue.Length == 0)
            _errors.Add(MinValueEmptyError);
        else if (minValue != null && minValue.Length > 50)
            _errors.Add(MinValueTooLongError.WithDetail($"min value: '{minValue}' (length: {minValue.Length})"));
    }

    private static void ValidateMaxValue(string? maxValue)
    {
        if (maxValue != null && maxValue.Length == 0)
            _errors.Add(MaxValueEmptyError);
        else if (maxValue != null && maxValue.Length > 50)
            _errors.Add(MaxValueTooLongError.WithDetail($"max value: '{maxValue}' (length: {maxValue.Length})"));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length == 0)
            _errors.Add(DescriptionEmptyError);
        else if (description != null && description.Length > 500)
            _errors.Add(DescriptionToLongError.WithDetail($"description length: {description.Length}"));
    }
}
