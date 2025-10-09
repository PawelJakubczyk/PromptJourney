using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence.Context;

public class MidjourneyDbContext : DbContext
{
    public DbSet<MidjourneyVersion> MidjourneyVersions { get; set; }
    public DbSet<MidjourneyProperties> MidjourneyProperties { get; set; }

    public DbSet<MidjourneyStyle> MidjourneyStyle { get; set; }
    public DbSet<MidjourneyPromptHistory> MidjourneyPromptHistory { get; set; }
    public DbSet<MidjourneyStyleExampleLink> MidjourneyStyleExampleLinks { get; set; }

    public MidjourneyDbContext(DbContextOptions<MidjourneyDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var persistenceAssembly = typeof(MidjourneyDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);
    }
}
