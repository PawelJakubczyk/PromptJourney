namespace Application.Features.Versions;

public class ParameterDetails
{
    public required string Version { get; set; }
    public required string PropertyName { get; set; }
    public string[]? Parameters { get; set; }
    public string? DefaultValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public string? Description { get; set; }
}