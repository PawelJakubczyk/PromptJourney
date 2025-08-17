using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;

using Microsoft.EntityFrameworkCore;

namespace Persistance.Context;

public class MidjourneyDbContext : DbContext
{
    public DbSet<MidjourneyVersions> MidjourneyVersionsMaster { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1> MidjourneyPropertiesVersion1 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2> MidjourneyPropertiesVersion2 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3> MidjourneyPropertiesVersion3 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4> MidjourneyPropertiesVersion4 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5> MidjourneyPropertiesVersion5 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51> MidjourneyPropertiesVersion51 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52> MidjourneyPropertiesVersion52 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6> MidjourneyPropertiesVersion6 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61> MidjourneyPropertiesVersion61 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7> MidjourneyPropertiesVersion7 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4> MidjourneyPropertiesVersionNiji4 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5> MidjourneyPropertiesVersionNiji5 { get; set; }
    public DbSet<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6> MidjourneyPropertiesVersionNiji6 { get; set; }
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
