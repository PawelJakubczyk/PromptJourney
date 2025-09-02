using Domain.Entities.MidjourneyStyles;
namespace Application.Features.ExampleLinks.Responses;

public sealed record ExampleLinkRespose
(
    string Link,
    string Style, 
    string Version
)
{
    public static ExampleLinkRespose FromDomain(MidjourneyStyleExampleLink exampleLink) =>
        new(exampleLink.Link.Value, exampleLink.Style.Name.Value, exampleLink.Version.Value);
}