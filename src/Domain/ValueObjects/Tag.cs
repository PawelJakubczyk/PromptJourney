using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Tag : ValueObject<string>, ICreatable<Tag, string?>
{
    public const int MaxLength = 50;

    private Tag(string value) : base(value) { }

    public static Result<Tag> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, Tag>(value)
            .IfLengthTooLong<DomainLayer, Tag>(value, MaxLength)
            .ExecuteIfNoErrors<Tag>(() => new Tag(value!))
            .MapResult<Tag>();

        return result;
    }
}
