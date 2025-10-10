using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

public class MidjourneyDbContext(DbContextOptions<MidjourneyDbContext> options) : DbContext(options)
{
    public DbSet<MidjourneyVersion> MidjourneyVersions { get; set; }
    public DbSet<MidjourneyProperties> MidjourneyProperties { get; set; }

    public DbSet<MidjourneyStyle> MidjourneyStyle { get; set; }
    public DbSet<MidjourneyPromptHistory> MidjourneyPromptHistory { get; set; }
    public DbSet<MidjourneyStyleExampleLink> MidjourneyStyleExampleLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var persistenceAssembly = typeof(MidjourneyDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);
    }
}