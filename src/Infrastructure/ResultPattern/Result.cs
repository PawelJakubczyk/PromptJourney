namespace Infrastructure.ResultPattern;

public class Result<T>
{
    public T? Value { get; private set; }
    public List<string> Errors { get; private set; } = new();
    public bool IsSuccess => Errors.Count == 0;

    public static Result<T> Success(T value) => new() { Value = value };
    public static Result<T> Failure(params string[] errors)
        => new()
        { Errors = [.. errors] };

    public void AddError(string error) => Errors.Add(error);
}
