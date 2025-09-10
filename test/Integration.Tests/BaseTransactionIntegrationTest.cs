using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Context;

namespace Integration.Tests;

[Collection("Database collection")]
public abstract class BaseTransactionIntegrationTest : IDisposable
{
    protected readonly MidjourneyDbContext DbContext;
    protected readonly MidjourneyDbFixture Fixture;
    private readonly IDbContextTransaction _transaction;
    private bool _disposed = false;

    protected BaseTransactionIntegrationTest(MidjourneyDbFixture fixture)
    {
        Fixture = fixture;
        DbContext = fixture.CreateDbContext();
        
        // Start a transaction that will be rolled back after each test
        // To w³aœnie gwarantuje pe³n¹ izolacjê bez usuwania bazy danych
        _transaction = DbContext.Database.BeginTransaction();
    }

    public virtual void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                // Rollback transaction instead of cleaning database
                // To jest bezpieczniejsze - wszystkie zmiany s¹ cofane
                _transaction.Rollback();
                _transaction.Dispose();
                
                // Clear change tracker
                DbContext.ChangeTracker.Clear();
                
                // Dispose the context
                DbContext.Dispose();
            }
            catch (Exception)
            {
                // Ignore cleanup errors during disposal
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}