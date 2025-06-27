using Application.Abstractions;
using Domain.Entities.MidjourneyVersions;
using Microsoft.EntityFrameworkCore;
using Persistans.Context;

namespace Persistance.Repositories;

public sealed class VersionRepository : IVersionRepository
{
    private readonly MidjourneyDbContext _midjourneyDbContext;

    public VersionRepository(MidjourneyDbContext midjourneyDbContext)
    {
        _midjourneyDbContext = midjourneyDbContext;
    }

    public async Task<MidjourneyVersionsMaster> GetMaterVersionByVersion(string version)
    {
        var versionMaster = await _midjourneyDbContext
            .MidjourneyVersionsMaster
            .Include(v => v.Versions1)
            .Include(v => v.Versions2)
            .Include(v => v.Versions3)
            .Include(v => v.Versions4)
            .Include(v => v.Versions5)
            .Include(v => v.Versions6)
            .Include(v => v.Versions61)
            .Include(v => v.Versions7)
            .Include(v => v.VersionsNiji4)
            .Include(v => v.VersionsNiji5)
            .Include(v => v.VersionsNiji6)
            .FirstOrDefaultAsync(v => v.Version == version);

        if (versionMaster == null)
        {
            throw new KeyNotFoundException($"Version '{version}' not found");
        }

        return versionMaster;
    }
}
