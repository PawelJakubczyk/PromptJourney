using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using static Persistence.Validate.Property;

namespace Persistence;

public class Validate
{



    public static class History
    {
    }


    public static class Property
    {

    }

    public static class Style
    {

        public static Task<Result<string>> ShouldBeNotNullOrEmpty(string style)
        {
            if (string.IsNullOrWhiteSpace(style))
                return Task.FromResult(Result.Fail<string>("Style cannot be null or empty."));

            return Task.FromResult(Result.Ok<string>("Style was found."));
        }

        public static Task<Result<string>> ShouldBeNotNull(MidjourneyStyle style)
        {
            if (style is null)
                return Task.FromResult(Result.Fail<string>("Style cannot be null."));

            return Task.FromResult(Result.Ok<string>("Style was found."));
        }

        public static Task<Result<string>> NameAndTypeShouldNotBeNullOrEmpty(MidjourneyStyle style)
        {
            if (string.IsNullOrEmpty(style.StyleName.Value))
                return Task.FromResult(Result.Fail<string>("Style name cannot be null or empty."));
            if (string.IsNullOrEmpty(style.Type.Value))
                return Task.FromResult(Result.Fail<string>("Style type cannot be null or empty."));
                
            return Task.FromResult(Result.Ok<string>("Style was found."));
        }
    }

    public static class Type
    {
        public static Task<Result<string>> ShouldBeNotNullOrEmpty(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return Task.FromResult(Result.Fail<string>("Type cannot be null or empty."));
            return Task.FromResult(Result.Ok<string>("Type was found."));
        }
    }

    public static class Tag
    {
        public static Task<Result<string>> ShouldBeNotNullOrEmpty(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return Task.FromResult(Result.Fail<string>("Tag cannot be null or empty."));
            return Task.FromResult(Result.Ok<string>("Tag was found."));
        }
    }
    

    public static class Tags
    {
        public static Task<Result<string>> ShouldNotHaveNullOrEmptyTag(List<string> tags)
        {
            foreach (string tag in tags)
            {
                if (string.IsNullOrWhiteSpace(tag))
                    return Task.FromResult(Result.Fail<string>("Tags cannot contain null or empty values."));
            }
            return Task.FromResult(Result.Ok<string>("Tags were found."));
        }

        public static Task<Result<string>> ShouldHaveAtLastOneElement(List<string> tags)
        {
            if (tags is null || tags.Count == 0)
                return Task.FromResult(Result.Fail<string>("Tags cannot be null or empty."));
            return Task.FromResult(Result.Ok<string>("Tags were found."));
        }
    }

    public static class Keyword
    {
        public static Task<Result<string>> ShouldBeNotNullOrEmpty(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Task.FromResult(Result.Fail<string>("Keyword cannot be null or empty."));

            return Task.FromResult(Result.Ok<string>("Keyword was found."));
        }
    }

    public static class Version
    {

    }
}