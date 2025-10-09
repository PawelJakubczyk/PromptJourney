using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Persistence.Context;

internal class MidjourneyDbContextFactory : IDesignTimeDbContextFactory<MidjourneyDbContext>
{
    public MidjourneyDbContext CreateDbContext(string[] args)
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