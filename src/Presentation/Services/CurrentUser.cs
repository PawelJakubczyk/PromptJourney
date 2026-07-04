using Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Presentation.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue("sub");

    public string? Role =>
        User?.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}