using BaliseManager.Api.Data;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaliseManager.Api.Repositories;

public sealed class BeaconEventRepository(ApplicationDbContext dbContext)
    : IBeaconEventRepository
{
    public async Task<IReadOnlyList<BeaconEvent>> GetByBeaconIdAsync(
        int beaconId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.BeaconEvents
            .AsNoTracking()
            .Where(beaconEvent => beaconEvent.BeaconId == beaconId)
            .OrderByDescending(beaconEvent => beaconEvent.OccurredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<BeaconEvent?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.BeaconEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(
                beaconEvent => beaconEvent.Id == id,
                cancellationToken);
    }

    public async Task<bool> MarkAsReadAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        BeaconEvent? beaconEvent = await dbContext.BeaconEvents
            .FirstOrDefaultAsync(
                eventItem => eventItem.Id == id,
                cancellationToken);

        if (beaconEvent is null)
        {
            return false;
        }

        beaconEvent.IsRead = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
