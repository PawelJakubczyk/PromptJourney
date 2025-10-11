namespace Application.UseCases.Common.Responses;

public sealed record DeleteResponse
(
    bool IsDeleted,
    string Message
)
{
    public static DeleteResponse Success(string message) =>
        new(true, message);

    public static DeleteResponse Failure(string message) =>
        new(false, message);
}