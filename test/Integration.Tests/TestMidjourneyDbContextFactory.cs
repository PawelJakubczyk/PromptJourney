using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;

namespace Integration.Tests;

public class TestMidjourneyDbContextFactory
{
    public MidjourneyDbContext CreateDbContext()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("connectionString.json")
            .Build();

        var connectionString = configuration.GetConnectionString("TestConnection");

        var optionsBuilder = new DbContextOptionsBuilder<MidjourneyDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new MidjourneyDbContext(optionsBuilder.Options);
    }
}