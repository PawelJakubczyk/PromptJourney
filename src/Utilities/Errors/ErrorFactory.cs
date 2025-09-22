using FluentResults;

namespace Application.Extension;

public static class ErrorFactory
{
    public static List<Error> EmptyErrors() => [];

    public static Task<List<Error>> EmptyErrorsAsync() =>
        Task.FromResult(new List<Error>());
}
