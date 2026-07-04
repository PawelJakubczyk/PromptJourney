using Application.Abstractions.Auth;
using Application.Abstractions.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Repositories.Utilities;
using Persistence.Services;

namespace Persistence.Registrations;

public static class PersistenceRegistration
{
    public static IServiceCollection RegisterPersistenceLayer
    (
        this IServiceCollection services,
        IHostEnvironment environment
    )
    {
        if (environment.EnvironmentName != "Testing")
        {
            services.AddDbContext<MidjourneyDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=midjourney_test;Username=admin_0;Password=GR52MqngWxfT");
            });
        }

        // Auth services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        // Caching
        services.AddMemoryCache();
        services.AddSingleton<HybridCache, DefaultHybridCache>();

        // Repositories
        services.AddScoped<IVersionRepository, VersionsRepository>();
        services.AddScoped<IStyleRepository, StylesRepository>();
        services.AddScoped<IExampleLinksRepository, ExampleLinkRepository>();
        services.AddScoped<IPropertiesRepository, PropertiesRepository>();
        services.AddScoped<IPromptHistoryRepository, PromptHistoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}