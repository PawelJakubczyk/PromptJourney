using Domain.ValueObjects;

namespace Application.Features.ExampleLinks.Responses;

public sealed record ExampleLinkRespose
(
    string Link,
    string Style, 
    string Version
)
{

}