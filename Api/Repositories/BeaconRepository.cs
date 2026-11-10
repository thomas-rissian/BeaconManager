using BaliseManager.Api.Data;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaliseManager.Api.Repositories;

public sealed class BeaconRepository(ApplicationDbContext dbContext)
    : IBeaconRepository
{
    public async Task<IReadOnlyList<Beacon>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Beacons
            .AsNoTracking()
            .OrderBy(beacon => beacon.Code)
            .ToListAsync(cancellationToken);
    }
}
