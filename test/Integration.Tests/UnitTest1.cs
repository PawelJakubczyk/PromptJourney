using Persistence.Context;

namespace Integration.Tests;

[Collection("Database collection")]
public class UnitTest1
{
    private readonly MidjourneyDbContext _dbContext;
    private readonly MidjourneyDbFixture _fixture;

    public UnitTest1(MidjourneyDbFixture fixture)
    {
        _fixture = fixture;
        _dbContext = fixture.DbContext;
        
        // Clean tables before each test
        _fixture.CleanupDatabase();
        
        // Set up test data here
        SetupTestData();
    }

    private void SetupTestData()
    {
        // Add your test data here
        // Example:
        // _dbContext.MidjourneyVersionsMaster.Add(new MidjourneyVersion(...));
        // _dbContext.SaveChanges();
    }

    [Fact]
    public void Test1()
    {
        // Verify database connection
        Assert.True(_dbContext.Database.CanConnect());
        
        // Write your test logic here
    }
}