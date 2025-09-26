using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record ExampleLink : ValueObject<string>, ICreatable<ExampleLink, string>
{
    public const int MaxLength = 200;

    private ExampleLink(string value) : base(value) { }

    public static Result<ExampleLink> Create(string value)
    {
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .IfNullOrWhitespace<DomainLayer, ExampleLink>(value)
                .IfLengthTooLong<DomainLayer, ExampleLink>(value, MaxLength)
                .IfLinkFormatInvalid<DomainLayer>(value))
            .ExecuteIfNoErrors<ExampleLink>(() => new ExampleLink(value))
            .MapResult(l => l);

        return result;
    }
}

internal static class ExampleLinkErrorsExtensions
{
    internal static WorkflowPipeline IfLinkFormatInvalid<TLayer>(this WorkflowPipeline pipeline, string value)
        where TLayer : ILayer
    {
        var isValid = Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                      (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!isValid)
        {
            pipeline.Errors.Add(new Error<TLayer>($"Invalid URL format: {value}", StatusCodes.Status400BadRequest));
        }

        return pipeline;
    }
}
