using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.Errors;

public static class DomainErrors
{
    public static Error NoAvailableVersionFound() =>
        ErrorFactories.NoAvailableExist<ModelVersion, DomainLayer>();

    public static Error VersionNotFound(ModelVersion modelVersion) =>
        ErrorFactories.NotFound<ModelVersion, DomainLayer>(modelVersion);

    public static Error ExampleLinkNotFound(Guid id) =>
        ErrorFactories.NotFound<Guid, DomainLayer>(id);

    public static Error HistoryNotFoundError(Guid historyId) =>
        ErrorFactories.NotFound<Guid, DomainLayer>(historyId);

    public static Error StyleNotFound(StyleName style) =>
        ErrorFactories.NotFound<StyleName, DomainLayer>(style);

    public static Error PropertyNotFound(PropertyName propertyName) =>
        ErrorFactories.NotFound<PropertyName, DomainLayer>(propertyName);
}
