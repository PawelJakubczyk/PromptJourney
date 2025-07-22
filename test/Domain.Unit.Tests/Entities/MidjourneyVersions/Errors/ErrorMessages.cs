namespace Domain.Unit.Tests.Entities.MidjourneyVersions.ErrorMassages;

public static class ErrorMessages
{
    // Validation messages
    public const string PropertyNameNullOrEmpty = "PropertyName cannot be null or empty";
    public const string PropertyNameTooLong = "PropertyName cannot exceed 25 characters";
    public const string VersionNullOrEmpty = "Version cannot be null or empty";
    public const string VersionTooLong = "Version cannot exceed 10 characters";
    public const string ParametersEmpty = "Parameters cannot be an empty array";
    public const string ParametersTooMany = "Parameters cannot exceed 10 items";
    public const string DefaultValueEmpty = "DefaultValue cannot be an empty string";
    public const string DefaultValueTooLong = "DefaultValue cannot exceed 50 characters";
    public const string MinValueEmpty = "MinValue cannot be an empty string";
    public const string MinValueTooLong = "MinValue cannot exceed 50 characters";
    public const string MaxValueEmpty = "MaxValue cannot be an empty string";
    public const string MaxValueTooLong = "MaxValue cannot exceed 50 characters";
    public const string DescriptionEmpty = "Description cannot be an empty string";
    public const string DescriptionTooLong = "Description cannot exceed 500 characters";
    public const string ReleaseDateInFuture = "ReleaseDate cannot be in the future";
    public const string ParameterTooLong = "Parameter cannot exceed 100 characters";
    public const string ParameterNullOrEmpty = "Parameter cannot be null or empty";
}