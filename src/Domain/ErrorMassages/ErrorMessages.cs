namespace Domain.ErrorMassages;

public static class ErrorMessages
{
    // Validation messages version
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

    // Validation messages for style
    public const string NameTooLong = "Name cannot exceed 50 characters";
    public const string TypeEmpty = "Type cannot be empty";
    public const string TypeTooLong = "Type cannot exceed 50 characters";
    public const string TagsEmpty = "Tags cannot be an empty array";
    public const string TagsTooMany = "Tags cannot exceed 10 items";
    public const string TagEmpty = "Tag cannot be null or empty";
    public const string TagTooLong = "Tag cannot exceed 50 characters";
    public const string ExampleLinksEmpty = "ExampleLinks cannot be an empty array";
    public const string ExampleLinksTooMany = "ExampleLinks cannot exceed 10 items";
    public const string ExampleLinkEmpty = "ExampleLink cannot be null or empty";
    public const string ExampleLinkTooLong = "ExampleLink cannot exceed 200 characters";
    public const string NameEmpty = "Name cannot be an empty string";

    // Validation messages for history
    public const string PromptNullOrEmpty = "Prompt cannot be null or empty";
    public const string PromptTooLong = "Prompt cannot exceed 500 characters";
}