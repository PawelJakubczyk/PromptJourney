using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyStyles;
//using Domain.Entities.MidjourneyVersions;
using Microsoft.EntityFrameworkCore;

namespace Persistans.Context;

public class MidjourneyDbContext : DbContext
{
    //public DbSet<MidjourneyVersionsMaster> MidjourneyVersionsMaster { get; set; }
    //public DbSet<MidjourneyVersion1> MidjourneyVersion1 { get; set; }
    //public DbSet<MidjourneyVersion2> MidjourneyVersion2 { get; set; }
    //public DbSet<MidjourneyVersion3> MidjourneyVersion3 { get; set; }
    //public DbSet<MidjourneyVersion4> MidjourneyVersion4 { get; set; }
    //public DbSet<MidjourneyVersion5> MidjourneyVersion5 { get; set; }
    //public DbSet<MidjourneyVersion51> MidjourneyVersion51 { get; set; }
    //public DbSet<MidjourneyVersion52> MidjourneyVersion52 { get; set; }
    //public DbSet<MidjourneyVersion6> MidjourneyVersion6 { get; set; }
    //public DbSet<MidjourneyVersion61> MidjourneyVersion61 { get; set; }
    //public DbSet<MidjourneyVersion7> MidjourneyVersion7 { get; set; }
    //public DbSet<MidjourneyVersionNiji4> MidjourneyVersionNiji4 { get; set; }
    //public DbSet<MidjourneyVersionNiji5> MidjourneyVersionNiji5 { get; set; }
    //public DbSet<MidjourneyVersionNiji6> MidjourneyVersionNiji6 { get; set; }
    public DbSet<MidjourneyStyle> MidjourneyStyle { get; set; }
    public DbSet<MidjourneyPromptHistory> MidjourneyPromptHistory { get; set; }

    public MidjourneyDbContext(DbContextOptions<MidjourneyDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var persistenceAssembly = typeof(MidjourneyDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);

        //modelBuilder.ApplyConfiguration(new Version1Configuration());
        //modelBuilder.ApplyConfiguration(new Version2Configuration());
        //modelBuilder.ApplyConfiguration(new Version3Configuration());
        //modelBuilder.ApplyConfiguration(new Version4Configuration());
        //modelBuilder.ApplyConfiguration(new Version5Configuration());
        //modelBuilder.ApplyConfiguration(new Version51Configuration());
        //modelBuilder.ApplyConfiguration(new Version52Configuration());
        //modelBuilder.ApplyConfiguration(new Version6Configuration());
        //modelBuilder.ApplyConfiguration(new Version61Configuration());
        //modelBuilder.ApplyConfiguration(new Version7Configuration());
        //modelBuilder.ApplyConfiguration(new VersionNiji4Configuration());
        //modelBuilder.ApplyConfiguration(new VersionNiji5Configuration());
        //modelBuilder.ApplyConfiguration(new VersionNiji6Configuration());
        //modelBuilder.ApplyConfiguration(new VersionsMasterConfiguration());
        //modelBuilder.ApplyConfiguration(new MidjourneyStyleConfiguration());
        //modelBuilder.ApplyConfiguration(new PromptHistoryConfiguration());
    }
}
