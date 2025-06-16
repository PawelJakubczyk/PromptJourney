using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;

using Microsoft.EntityFrameworkCore;

namespace Persistans.Context;

public class MidjourneyDbContext : DbContext
{
    public DbSet< MidjourneyVersionsMaster> MidjourneyVersionsMaster { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion1> MidjourneyVersion1 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion2> MidjourneyVersion2 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion3> MidjourneyVersion3 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion4> MidjourneyVersion4 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion5> MidjourneyVersion5 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion51> MidjourneyVersion51 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion52> MidjourneyVersion52 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion6> MidjourneyVersion6 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion61> MidjourneyVersion61 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersion7> MidjourneyVersion7 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersionNiji4> MidjourneyVersionNiji4 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersionNiji5> MidjourneyVersionNiji5 { get; set; }
    public DbSet<MidjourneyAllVersion.MidjourneyVersionNiji6> MidjourneyVersionNiji6 { get; set; }
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
