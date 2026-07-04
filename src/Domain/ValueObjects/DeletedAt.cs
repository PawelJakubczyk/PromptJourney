using Domain.Abstractions;
using Domain.Extensions;
using System.Globalization;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record DeletedAt : ValueObject<DateTimeOffset>, ICreatable<DeletedAt, string?>
{
    public static readonly DeletedAt None = new(DateTimeOffset.MinValue);
    public override bool IsNone => this == None;

    private DeletedAt(DateTimeOffset value) : base(value) { }

    public static Result<DeletedAt> Create(string? value)
    {
        value = value?.Trim();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(None);

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DeletedAt>(value)
            .IfDateFormatInvalid<DeletedAt>(value!)
            .ExecuteIfNoErrors<DeletedAt>(() => new DeletedAt(
                DateTimeOffset.Parse(value!, null, DateTimeStyles.AssumeUniversal)))
            .MapResult<DeletedAt>();

        return result;
    }

    public static Result<DeletedAt> Create() => Result.Ok(new DeletedAt(DateTimeOffset.UtcNow));
}

