using Domain.Entities.MidjourneyVersions;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Entities.MidjourneyStyles;

public class MidjourneyStyleExampleLink
{
    // Primary key
    public string Link { get; private set; }
    
    // Foreign keys
    public string StyleName { get; private set; }
    public string Version { get; private set; }
    
    // Navigation
    public MidjourneyStyle Style { get; private set; } = null!;
    public MidjourneyVersionsMaster VersionMaster { get; private set; } = null!;

    // Errors
    private static List<DomainError> _errors = [];

    // Constructors
    private MidjourneyStyleExampleLink() 
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyStyleExampleLink(string link, string styleName, string version)
    {
        Link = link;
        StyleName = styleName;
        Version = version;
    }
    
    public static Result<MidjourneyStyleExampleLink> Create(
        string link, 
        string styleName, 
        string version)
    {
        var errors = new List<Error>();
        
        ValidateLink(link);
        ValidateName(styleName);
        ValidateVersion(version);

        if (errors.Any())
            return Result.Fail<MidjourneyStyleExampleLink>(errors);
            
        return Result.Ok(new MidjourneyStyleExampleLink(link, styleName, version));
    }

    // Validation methods
    private static void ValidateLink(string link)
    {
        if (string.IsNullOrWhiteSpace(link))
            _errors.Add(ExampleLinkNullOrEmptyError);
        else if (link.Length > 200)
            _errors.Add(ExampleLinkTooLongError.WithDetail($"link length: {link.Length}"));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            _errors.Add(NameNullOrEmptyError);
        else if (name.Length > 100)
            _errors.Add(NameToLongError.WithDetail($"name length: {name.Length}"));
    }

    private static void ValidateVersion(string version) {
        if (string.IsNullOrWhiteSpace(version))
            _errors.Add(VersionNullOrEmptyError);
        else if (version.Length > 10)
            _errors.Add(VersionToLongError.WithDetail($"version length: {version.Length}"));
    }


    //private static void ValidateExampleLink(ICollection<string>? exampleLinks)
    //{
    //    if (exampleLinks?.Count > 10)
    //        _errors.Add(ExampleLinksTooManyError.WithDetail($"link count: {exampleLinks.Count}"));

    //    foreach (var link in exampleLinks!)
    //    {
    //        if (link.Length > 200)
    //            _errors.Add(ExampleLinkTooLongError.WithDetail($"link length: {link.Length}"));
    //    }
    //}

    //private static void ValidateExampleLinkExists(ICollection<string> exampleLinks, string expectedLink)
    //{
    //    if (!exampleLinks.Contains(expectedLink))
    //    {
    //        _errors.Add(ExampleLinkNotFoundError.WithDetail($"link: '{expectedLink}'"));
    //    }
    //}

    //private static void ValidateExampleLinkNotExists(ICollection<string> exampleLinks, string expectedLink)
    //{
    //    if (exampleLinks.Contains(expectedLink))
    //    {
    //        _errors.Add(ExampleLinkDuplicateError.WithDetail($"link: '{expectedLink}'"));
    //    }    //public Result<MidjourneyStyle>? AddLink(string link)
    //{
    //    ValidateExampleLinks([link]);
    //    ValidateExampleLinkNotExists(ExampleLinks, link);

    //    if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

    //    ExampleLinks.Add(link);
    //    return Result.Ok(this);
    //}

    //public Result<MidjourneyStyle>? RemoveLink(string link)
    //{
    //    ValidateExampleLinks([link]);
    //    ValidateExampleLinkExists(ExampleLinks, link);

    //    if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

    //    ExampleLinks.Remove(link);
    //    return Result.Ok(this);
    //}
    //}


}