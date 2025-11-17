using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record ExampleLink : ValueObject<string>, ICreatable<ExampleLink, string?>
{
    public const int MaxLength = 200;

    private ExampleLink(string value) : base(value) { }

    public static Result<ExampleLink> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, ExampleLink>(value)
            .Congregate(pipeline => pipeline
                .IfLengthTooLong<DomainLayer, ExampleLink>(value, MaxLength)
                .IfLinkFormatInvalid<DomainLayer>(value))
            .ExecuteIfNoErrors<ExampleLink>(() => new ExampleLink(value))
            .MapResult<ExampleLink>();

        return result;
    }
}

internal static class ExampleLinkErrorsExtensions
{
    internal const string InvalidUrlFormatMessage = $"Invalid URL format";

    internal static WorkflowPipeline IfLinkFormatInvalid<TLayer>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
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
                ErrorFactories.InvalidPattern<string, TLayer>(value, InvalidUrlFormatMessage)
            );
        }

        return pipeline;
    }
}
