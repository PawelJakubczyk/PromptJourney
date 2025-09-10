//using Microsoft.EntityFrameworkCore;
//using Persistence.Context;

//namespace Integration.Tests;

//[Collection("Database collection")]
//public abstract class BaseIntegrationTest : IDisposable
//{
//    protected readonly MidjourneyDbContext DbContext;
//    protected readonly MidjourneyDbFixture Fixture;
//    private bool _disposed = false;

//    protected BaseIntegrationTest(MidjourneyDbFixture fixture)
//    {
//        Fixture = fixture;
//        DbContext = fixture.CreateDbContext();
        
//        // Clean database before each test
//        fixture.CleanupDatabase();
//    }

//    public virtual void Dispose()
//    {
//        if (!_disposed)
//        {
//            try
//            {
//                // Clear change tracker to avoid entity tracking issues
//                DbContext.ChangeTracker.Clear();
                
//                // Dispose the context
//                DbContext.Dispose();
                
//                // Clean database after test (only truncate tables, don't delete database)
//                Fixture.CleanupDatabase();
//            }
//            catch (Exception)
//            {
//                // Ignore cleanup errors during disposal
//            }
//            finally
//            {
//                _disposed = true;
//            }
//        }
//    }
//}