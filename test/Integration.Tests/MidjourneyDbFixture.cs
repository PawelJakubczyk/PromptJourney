using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Integration.Tests;

public class MidjourneyDbFixture : IDisposable
{
    public MidjourneyDbContext DbContext { get; }

    public MidjourneyDbFixture()
    {
        var factory = new TestMidjourneyDbContextFactory();
        DbContext = factory.CreateDbContext();
        
        // Initialize the database schema
        DbContext.Database.EnsureCreated();
    }

    public void CleanupDatabase()
    {
        // PostgreSQL-specific cleanup that truncates all tables but keeps schema
        DbContext.Database.ExecuteSqlRaw
        (@"
            DO $$ DECLARE
                r RECORD;
            BEGIN
                FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = current_schema()) LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE;';
                END LOOP;
            END $$;
        ");
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<MidjourneyDbFixture>
{
    // This class is just a marker for the collection definition
}