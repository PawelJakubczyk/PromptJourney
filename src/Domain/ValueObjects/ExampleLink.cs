using Domain.Abstractions;
using Utilities.Errors;
using Utilities.Workflows;
using Utilities.Results;
using Domain.Extensions;

namespace Domain.ValueObjects;

public record ExampleLink : ValueObject<string>, ICreatable<ExampleLink, string>
{
    public const int MaxLength = 200;
    public override bool IsNone => false;

    private ExampleLink(string value) : base(value) { }


    public static Result<ExampleLink> Create(string value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<ExampleLink>(value)
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<ExampleLink>(value!, MaxLength),
                pipeline => pipeline.IfLinkFormatInvalid(value!))
            .ExecuteIfNoErrors<ExampleLink>(() => new ExampleLink(value!))
            .MapResult<ExampleLink>();

        return result;
    }
}

internal static class ExampleLinkErrorsExtensions
{
    internal const string InvalidUrlFormatMessage = $"Invalid URL format";

    internal static WorkflowPipeline IfLinkFormatInvalid(
        this WorkflowPipeline pipeline,
        string? value)
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (string.IsNullOrWhiteSpace(value))
            return pipeline;

        var decodedValue = Uri.UnescapeDataString(value);

        var isValid = Uri.TryCreate(decodedValue, UriKind.Absolute, out var uri) &&
                      (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!isValid)
        {
            pipeline.Errors.Add(
                ErrorFactories.InvalidPattern<string>(value, InvalidUrlFormatMessage)
            );
        }

        return pipeline;
    }
}
