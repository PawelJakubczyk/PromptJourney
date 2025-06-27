namespace Presentation.Requests;

public sealed record AddParameterToVersionRequest
{
    public required string PropertyName { get; init; }
    public required string[] Parameters { get; init; }
    public string? DefaultValue { get; init; }
    public string? MinValue { get; init; }
    public string? MaxValue { get; init; }
    public string? Description { get; init; }
}