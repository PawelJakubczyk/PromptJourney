using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using Persistence.Context;
using Application.Features;
using Application.Abstractions.IRepository;
using Persistence.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Integration.Tests;

[CollectionDefinition("Integration Tests")]
public class IntegrationTestsCollection : ICollectionFixture<MidjourneyTestWebApplicationFactory>
{
}

public class MidjourneyTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add core services
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(Application.Features.ExampleLinks.Commands.AddExampleLink).Assembly);
            });
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MidjourneyDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database
            services.AddDbContext<MidjourneyDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Add other required services
            services.AddLogging();

            // Register repositories
            services.AddScoped<IExampleLinksRepository, ExampleLinkRepository>();
            services.AddScoped<IStyleRepository, StylesRepository>();
            services.AddScoped<IVersionRepository, VersionsRepository>();
            services.AddScoped<IPropertiesRepository, PropertiesRepository>();
            services.AddScoped<IPromptHistoryRepository, PromptHistoryRepository>();
        });
    }
}