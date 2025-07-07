namespace Presentation.Requests;

public sealed record PatchParameterInVersionRequest
{
    public required string PropertyToUpdate { get; init; }
    public string? NewValue { get; init; }
}