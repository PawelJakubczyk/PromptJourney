using Microsoft.Extensions.DependencyInjection;

namespace Application.Registrations;

public class MyMarker { }

public static class ApplicationRegistration
{
    public static IServiceCollection RegisterApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<MyMarker>());
        return services;
    }
}