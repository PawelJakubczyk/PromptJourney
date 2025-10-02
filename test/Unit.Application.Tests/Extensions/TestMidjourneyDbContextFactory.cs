//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Persistence;

//namespace Integration.Tests;

//public class TestMidjourneyDbContextFactory
//{
//    private readonly string _connectionString;

//    public TestMidjourneyDbContextFactory()
//    {
//        var configuration = new ConfigurationBuilder()
//            .AddJsonFile("connectionString.json")
//            .Build();

//        _connectionString = configuration.GetConnectionString("TestConnection")
//            ?? throw new InvalidOperationException("TestConnection string not found");
//    }

//    public string GetConnectionString() => _connectionString;

//    public MidjourneyDbContext CreateDbContext()
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<MidjourneyDbContext>();

//        // Configure for testing - important options
//        optionsBuilder
//            .UseNpgsql(_connectionString)
//            .EnableSensitiveDataLogging() // Helpful for debugging
//            .EnableDetailedErrors(); // Better error messages

//        return new MidjourneyDbContext(optionsBuilder.Options);
//    }
//}