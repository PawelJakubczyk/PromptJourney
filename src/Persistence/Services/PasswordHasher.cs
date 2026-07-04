using Application.Abstractions.Auth;

namespace Persistence.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string hash, string password) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
