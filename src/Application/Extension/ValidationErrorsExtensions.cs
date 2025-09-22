using Application.Abstractions.IRepository;
using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Application.Extension;

public static class ValidationErrorsExtensions
{
    public static Task<List<Error>> IfVersionNotExists
    (
        this Task<List<Error>> errors,
        ModelVersion version,
        IVersionRepository repo,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        return errors.ValidateExistence
        (
            version,
            repo.CheckVersionExistsInVersionsAsync,
            "Version",
            shouldExist: true,
            cancellationToken,
            breakIfError
        );
    }

    public static Task<List<Error>> IfVersionAlreadyExists
    (
        this Task<List<Error>> errors,
        ModelVersion version,
        IVersionRepository repo,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        return errors.ValidateExistence
        (
            version,
            repo.CheckVersionExistsInVersionsAsync,
            "Version",
            shouldExist: false,
            cancellationToken,
            breakIfError
        );
    }

    public static Task<List<Error>> IfStyleNotExists
    (
        this Task<List<Error>> errors,
        StyleName style,
        IStyleRepository repo,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        return errors.ValidateExistence
        (
            style,
            repo.CheckStyleExistsAsync,
            "Style",
            shouldExist: true,
            cancellationToken,
            breakIfError
        );
    }

    public static Task<List<Error>> IfStyleAlreadyExists
    (
        this Task<List<Error>> errors,
        StyleName style,
        IStyleRepository repo,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        return errors.ValidateExistence
        (
            style,
            repo.CheckStyleExistsAsync,
            "Style",
            shouldExist: false,
            cancellationToken,
            breakIfError
        );
    }

    public static Task<List<Error>> IfLinkNotExists
    (
        this Task<List<Error>> errors,
        ExampleLink link,
        IExampleLinksRepository repo,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        return errors.ValidateExistence
        (
            link,
            repo.CheckExampleLinkExistsAsync,
            "Link",
            shouldExist: true,
            cancellationToken,
            breakIfError
        );
    }

    public static Task<List<Error>> IfLinkAlreadyExists
    (
        this Task<List<Error>> errors,
        ExampleLink link,
        IExampleLinksRepository repo,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        return errors.ValidateExistence
        (
            link,
            repo.CheckExampleLinkExistsAsync,
            "Link",
            shouldExist: false,
            cancellationToken,
            breakIfError
        );
    }

    public static Task<List<Error>> IfTagAlreadyExists
    (
        this Task<List<Error>> errorsTask,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        if (breakIfError && errorsTask.Result.Count != 0)
            return errorsTask;

        return errorsTask.ValidateExistence(
            tag,
            (tag, cancellationToken) => repository.CheckTagExistsInStyleAsync(styleName, tag, cancellationToken),
            "Tag",
            shouldExist: false,
            cancellationToken
        );
    }

    public static Task<List<Error>> IfTagNotExist
    (
        this Task<List<Error>> errorsTask,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
    {
        if (breakIfError && errorsTask.Result.Count != 0)
            return errorsTask;

        return errorsTask.ValidateExistence(
            tag,
            (tag, cancellationToken) => repository.CheckTagExistsInStyleAsync(styleName, tag, cancellationToken),
            "Tag",
            shouldExist: true,
            cancellationToken
        );
    }

    public static async Task<List<Error>> IfDateInFuture
    (
        this Task<List<Error>> errorsTask, 
        DateTime date,
        bool breakIfError = false
    )
    {
        var errors = await errorsTask;
        if (breakIfError && errors.Count != 0)
            return errors;

        if (date > DateTime.UtcNow)
        {
            errors.Add(new Error<DomainLayer>($"Date '{date:yyyy-MM-dd}' cannot be in the future."));
            return errors;
        }

        return errors;
    }

    public static async Task<List<Error>> IfDateRangeNotChronological
    (
        this Task<List<Error>> errorsTask, 
        DateTime from, 
        DateTime to, 
        bool breakIfError = false
    )
    {
        var errors = await errorsTask;
        if (breakIfError && errors.Count != 0)
            return errors;

        if (from > to)
        {
            errors.Add(new Error<DomainLayer>($"Date range is not chronological: 'From' ({from:yyyy-MM-dd}) is after 'To' ({to:yyyy-MM-dd})."));
            return errors;
        }

        return errors;
    }

    public static async Task<List<Error>> IfHistoryLimitNotGreaterThanZero
    (
        this Task<List<Error>> errorsTask, 
        int count, 
        bool breakIfError = false
    )
    {
        var errors = await errorsTask;
        if (breakIfError && errors.Count != 0)
            return errors;

        if (count <= 0)
        {
            errors.Add(new Error($"History count must be greater than zero. Provided: {count}."));
        }

        return errors;
    }

    public static async Task<List<Error>> IfListIsNullOrEmpty<TValue>
    (
        this Task<List<Error>> errorsTask, 
        List<TValue>? items, 
        bool breakIfError = false
    )
    {
        var errors = await errorsTask;
        if (breakIfError && errors.Count != 0)
            return errors;

        if (items is null || !(items.Count == 0))
        {
            var name = typeof(TValue).Name;
            errors.Add(new Error($"List of '{name}' must not be empty."));
        }
        return errors;
    }

public static async Task<List<Error>> IfHistoryCountExceedsAvailable
    (
        this Task<List<Error>> errorsTask, 
        int requestedCount, 
        IPromptHistoryRepository repository, 
        CancellationToken cancellationToken, 
        bool breakIfError = false
    )
    {
        var errors = await errorsTask;
        if (breakIfError && errors.Count != 0)
            return errors;

        var availableCountResult = await repository.CalculateHistoricalRecordCountAsync(cancellationToken);

        if (availableCountResult.IsFailed)
        {
            errors.AddRange(availableCountResult.Errors.OfType<Error>());
            return errors;
        }

        if (requestedCount > availableCountResult.Value)
        {
            errors.Add(new Error($"Requested {requestedCount} records, but only {availableCountResult.Value} are available."));
        }

        return errors;
    }

    private static async Task<List<Error>> ValidateExistence<TType>
    (
        this Task<List<Error>> errorsTask,
        TType item,
        Func<TType, CancellationToken, Task<Result<bool>>> existsFunc,
        string entityName,
        bool shouldExist,
        CancellationToken cancellationToken,
        bool breakIfError = false
    )
        where TType : ValueObject<string>
    {
        var errors = await errorsTask;
        if (breakIfError && errors.Count != 0)
            return errors;

        var result = await existsFunc(item, cancellationToken);

        if (result.IsFailed)
            errors.Add(new Error<PersistenceLayer>($"Failed to check if {entityName} exists"));
        if (result.Value != shouldExist)
            errors.Add(new Error<ApplicationLayer>($"{entityName} '{item}' {(shouldExist ? "not found" : "already exists")}"));

        return errors;
    }
}
