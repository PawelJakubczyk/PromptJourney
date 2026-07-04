using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record PasswordHash : ValueObject<string>, ICreatable<PasswordHash, string?>
{
    public const int MaxLength = 512;
    public override bool IsNone => false;
    private PasswordHash(string value) : base(value) { }

    public static Result<PasswordHash> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<PasswordHash>(value)
            .IfLengthTooLong<PasswordHash>(value!, MaxLength)
            .ExecuteIfNoErrors<PasswordHash>(() => new PasswordHash(value!))
            .MapResult<PasswordHash>();

        return result;
    }
}