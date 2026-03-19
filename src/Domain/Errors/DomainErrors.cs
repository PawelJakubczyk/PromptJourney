using Domain.ValueObjects;
using Utilities.Errors;

namespace Domain.Errors;

public static class DomainErrors
{
    public static Error NoAvailableVersionFound() =>
        ErrorFactories.NoAvailableExist<ModelVersion>();

    public static Error VersionNotFound(ModelVersion modelVersion) =>
        ErrorFactories.NotFound<ModelVersion>(modelVersion);

    public static Error ExampleLinkNotFound(Guid id) =>
        ErrorFactories.NotFound<Guid>(id);

    public static Error HistoryNotFoundError(Guid historyId) =>
        ErrorFactories.NotFound<Guid>(historyId);

    public static Error StyleNotFound(StyleName style) =>
        ErrorFactories.NotFound<StyleName>(style);

    public static Error PropertyNotFound(PropertyName propertyName) =>
        ErrorFactories.NotFound<PropertyName>(propertyName);

    public static Error TagNotFound(Tag tag) =>
        ErrorFactories.NotFound<Tag>(tag);

    public static Error InvalidReleaseDateFormat(string date) =>
        ErrorFactories.InvalidDateFormat<ReleaseDate>(date);

    public static Error InvalidStyleTypeNotAllowed(string value, Type enumType) =>
        ErrorFactories.OptionNotAllowed<StyleType>(value, enumType);

    public static Error InvalidUpdatePropertyNotFound(string propertyName) =>
        ErrorFactories.NotFound<string>(propertyName);
}
