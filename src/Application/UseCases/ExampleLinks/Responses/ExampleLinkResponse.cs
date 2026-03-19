using Domain.Entities;

namespace Application.UseCases.ExampleLinks.Responses;

public sealed record ExampleLinkResponse
(
    Guid Id,
    string Link,
    string Style,
    string Version
)
{
    public static ExampleLinkResponse FromDomain(MidjourneyStyleExampleLink exampleLink) =>
    new(
        exampleLink.Id.Value,
        exampleLink.Link.Value, 
        exampleLink.MidjuorneyStyle.StyleName.Value, 
        exampleLink.Version.Value
        );
}
