using Domain.Entities;

namespace Application.UseCases.ExampleLinks.Responses;

public sealed record ExampleLinkResponse
(
    string Link,
    string Style,
    string Version
)
{
    public static ExampleLinkResponse FromDomain(MidjourneyStyleExampleLink exampleLink) =>
        new(exampleLink.Link.Value, exampleLink.MidjuorneyStyle.StyleName.Value, exampleLink.Version.Value);
}
