namespace Application.Abstractions.Auth;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
