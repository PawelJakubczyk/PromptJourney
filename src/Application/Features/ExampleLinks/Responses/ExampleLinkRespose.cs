using Domain.ValueObjects;

namespace Application.Features.ExampleLinks.Responses;

public sealed record ExampleLinkRespose
(
    ExampleLink Link,
    StyleName Style, 
    ModelVersion Version
)
{

}