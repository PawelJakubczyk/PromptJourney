using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Application.Abstractions.IRepository;
using Persistence.Repositories;

namespace Persistence.Registrations;

public static class PersistenceRegistration
{
    public static IServiceCollection RegisterPersistenceLayer(this IServiceCollection services)
    {
        services.AddDbContext<MidjourneyDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=midjourney_test;Username=admin_0;Password=GR52MqngWxfT");
        });

        // Register repositories
        services.AddScoped<IVersionRepository, VersionsRepository>();
        services.AddScoped<IStyleRepository, StylesRepository>();
        services.AddScoped<IExampleLinksRepository, ExampleLinkRepository>();
        services.AddScoped<IPropertiesRepository, PropertiesRepository>();
        services.AddScoped<IPromptHistoryRepository, PromptHistoryRepository>();

        return services;
    }
}