using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Repositories.Versions;
using Persistans.Context;

namespace Persistance.Registrations;

public static class PersistenceRegistration
{
    public static IServiceCollection RegisterPersistenceLayer(this IServiceCollection services)
    {
        services.AddDbContext<MidjourneyDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=midjourney_test;Username=admin_0;Password=GR52MqngWxfT");
        });

        services.AddScoped<IVersionRepository, VersionsRepository>();
        return services;
    }
}