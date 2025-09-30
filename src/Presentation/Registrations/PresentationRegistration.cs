using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Registrations;

public static class PresentationRegistration
{
    public static IServiceCollection RegisterPresentationLayer(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static IApplicationBuilder UsePresentationLayer(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        return app;
    }
}