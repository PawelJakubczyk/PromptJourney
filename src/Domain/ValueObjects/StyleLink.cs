namespace Domain.ValueObjects;

public record StyleLink(string Url)
{
    public bool IsValid()
    {
        return Uri.TryCreate(Url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
