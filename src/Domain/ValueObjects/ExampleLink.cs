using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.ValueObjects;

public record ExampleLink : ValueObject<string>, ICreatable<ExampleLink, string>
{
    public const int MaxLength = 200;

    private ExampleLink(string value) : base(value) { }

    public static Result<ExampleLink> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, ExampleLink>(value)
            .IfLengthTooLong<DomainLayer, ExampleLink>(value, MaxLength)
            .IfLinkFormatInvalid<DomainLayer>(value);

        if (errors.Count != 0)
            return Result.Fail<ExampleLink>(errors);

        return Result.Ok(new ExampleLink(value));
    }
}

internal static class ExampleLinkErrorsExtensions
{
    internal static List<Error> IfLinkFormatInvalid<TLayer>(this List<Error> domainErrors, string value)
        where TLayer : ILayer
    {
        var isValid = Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!isValid)
        {
            domainErrors.Add(new Error<TLayer>($"Invalid URL format: {value}"));
        }

        return domainErrors;
    }
}