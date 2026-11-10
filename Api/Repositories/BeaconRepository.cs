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

    public async Task<Beacon?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Beacons
            .AsNoTracking()
            .FirstOrDefaultAsync(beacon => beacon.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        // The unique database index remains authoritative; this pre-check
        // allows the service to return a clear conflict in normal requests.
        return dbContext.Beacons
            .AsNoTracking()
            .AnyAsync(
                beacon => beacon.Code == code &&
                    (!excludingId.HasValue || beacon.Id != excludingId.Value),
                cancellationToken);
    }

    public async Task AddAsync(
        Beacon beacon,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Beacons.AddAsync(beacon, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Beacon beacon,
        CancellationToken cancellationToken = default)
    {
        dbContext.Beacons.Update(beacon);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
