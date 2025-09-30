using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Integration.Tests;

public class MidjourneyDbFixture : IDisposable
{
    private readonly TestMidjourneyDbContextFactory _factory;
    private bool _disposed = false;

    public MidjourneyDbFixture()
    {
        _factory = new TestMidjourneyDbContextFactory();
        
        // Create the database schema once for all tests
        using var context = CreateDbContext();
        
        // Ensure database exists but don't recreate if it already exists
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }
    }

    public MidjourneyDbContext CreateDbContext()
    {
        return _factory.CreateDbContext();
    }

    public void CleanupDatabase()
    {
        using var context = CreateDbContext();
        
        // PostgreSQL-specific cleanup that truncates all tables but keeps schema
        context.Database.ExecuteSqlRaw
        (@"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                -- Disable triggers to avoid constraint issues during truncation
                SET session_replication_role = replica;
                
                FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = current_schema()) LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE;';
                END LOOP;
                
                -- Re-enable triggers
                SET session_replication_role = DEFAULT;
            END $$;
        ");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<MidjourneyDbFixture>
{

}