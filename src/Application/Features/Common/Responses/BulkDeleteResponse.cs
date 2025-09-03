namespace Application.Features.Common.Responses;

public sealed record BulkDeleteResponse
(
    bool IsDeleted,
    int DeletedCount,
    string Message
)
{
    public static BulkDeleteResponse Success(int deletedCount, string message) =>
        new(true, deletedCount, message);

    public static BulkDeleteResponse Failure(string message) =>
        new(false, 0, message);
}