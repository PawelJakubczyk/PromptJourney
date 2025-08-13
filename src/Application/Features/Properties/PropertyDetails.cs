namespace Application.Features.Properties;

public class PropertyDetails
{
    public required string Version { get; set; }
    public required string PropertyName { get; set; }
    public string[]? Parameters { get; set; }
    public string? DefaultValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public string? Description { get; set; }
}