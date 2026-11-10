using BaliseManager.Api.Data;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaliseManager.Api.Repositories;

public sealed class DashboardRepository(ApplicationDbContext dbContext)
    : IDashboardRepository
{
    public async Task<IReadOnlyList<Beacon>> GetBeaconsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Beacons
            .AsNoTracking()
            .OrderBy(beacon => beacon.Code)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetUnreadEventCountAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.BeaconEvents
            .AsNoTracking()
            .CountAsync(beaconEvent => !beaconEvent.IsRead, cancellationToken);
    }

    public async Task<IReadOnlyList<BeaconEvent>> GetRecentEventsAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.BeaconEvents
            .AsNoTracking()
            .OrderByDescending(beaconEvent => beaconEvent.OccurredAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
