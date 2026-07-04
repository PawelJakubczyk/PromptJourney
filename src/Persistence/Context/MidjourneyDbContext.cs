using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

public class MidjourneyDbContext(DbContextOptions<MidjourneyDbContext> options) : DbContext(options)
{
    public DbSet<MidjourneyVersion> MidjourneyVersions { get; set; }
    public DbSet<MidjourneyProperty> MidjourneyProperties { get; set; }
    public DbSet<MidjourneyStyle> MidjourneyStyles { get; set; }
    public DbSet<MidjourneyPromptHistory> MidjourneyPromptHistories { get; set; }
    public DbSet<MidjourneyStyleExampleLink> MidjourneyStyleExampleLinks { get; set; }
    public DbSet<MidjourneyUser> MidjourneyUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var persistenceAssembly = typeof(MidjourneyDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);
    }
}