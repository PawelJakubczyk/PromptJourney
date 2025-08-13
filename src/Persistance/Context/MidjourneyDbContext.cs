using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;

using Microsoft.EntityFrameworkCore;

namespace Persistans.Context;

public class MidjourneyDbContext : DbContext
{
    public DbSet<MidjourneyVersionsMaster> MidjourneyVersionsMaster { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1> MidjourneyVersion1 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2> MidjourneyVersion2 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3> MidjourneyVersion3 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4> MidjourneyVersion4 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5> MidjourneyVersion5 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51> MidjourneyVersion51 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52> MidjourneyVersion52 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6> MidjourneyVersion6 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61> MidjourneyVersion61 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7> MidjourneyVersion7 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4> MidjourneyVersionNiji4 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5> MidjourneyVersionNiji5 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6> MidjourneyVersionNiji6 { get; set; }
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
