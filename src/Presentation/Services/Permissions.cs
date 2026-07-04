using Microsoft.Extensions.DependencyInjection;
using static Domain.ValueObjects.Role;

namespace Presentation.Services;

public static class Permissions
{
    public static IServiceCollection AddPresentationAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(UserAccess, policy => policy.RequireRole(
                    User,
                    Moderator,
                    Admin))
            .AddPolicy(ModeratorAccess, policy => policy.RequireRole(
                    Moderator,
                    Admin))
            .AddPolicy(AdminAccess, policy => policy.RequireRole(
                    Admin));

        return services;
    }
}
