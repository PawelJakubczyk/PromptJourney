using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;

using Microsoft.EntityFrameworkCore;

namespace Persistans.Context;

public class MidjourneyDbContext : DbContext
{
    public DbSet<MidjourneyVersionsMaster> MidjourneyVersionsMaster { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion1> MidjourneyVersion1 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion2> MidjourneyVersion2 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion3> MidjourneyVersion3 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion4> MidjourneyVersion4 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion5> MidjourneyVersion5 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion51> MidjourneyVersion51 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion52> MidjourneyVersion52 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion6> MidjourneyVersion6 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion61> MidjourneyVersion61 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersion7> MidjourneyVersion7 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersionNiji4> MidjourneyVersionNiji4 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersionNiji5> MidjourneyVersionNiji5 { get; set; }
    public DbSet<MidjourneyAllVersions.MidjourneyVersionNiji6> MidjourneyVersionNiji6 { get; set; }
    public DbSet<MidjourneyStyle> MidjourneyStyle { get; set; }
    public DbSet<MidjourneyPromptHistory> MidjourneyPromptHistory { get; set; }

    public MidjourneyDbContext(DbContextOptions<MidjourneyDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var persistenceAssembly = typeof(MidjourneyDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);
    }
}
