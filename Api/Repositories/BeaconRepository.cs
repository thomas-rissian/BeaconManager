using BaliseManager.Api.Data;
using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaliseManager.Api.Repositories;

public sealed class BeaconRepository(ApplicationDbContext dbContext)
    : IBeaconRepository
{
    public async Task<(IReadOnlyList<Beacon> Beacons, int TotalCount)> GetPageAsync(
        BeaconQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Beacon> beaconQuery = dbContext.Beacons
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string searchPattern = $"%{query.Search.Trim()}%";
            beaconQuery = beaconQuery.Where(beacon =>
                EF.Functions.Like(beacon.Code, searchPattern) ||
                EF.Functions.Like(beacon.Name, searchPattern) ||
                (beacon.Equipment != null &&
                    EF.Functions.Like(beacon.Equipment, searchPattern)) ||
                (beacon.Site != null &&
                    EF.Functions.Like(beacon.Site, searchPattern)));
        }

        if (query.Status.HasValue)
        {
            beaconQuery = beaconQuery.Where(beacon =>
                beacon.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Site))
        {
            string site = query.Site.Trim();
            beaconQuery = beaconQuery.Where(beacon => beacon.Site == site);
        }

        int totalCount = await beaconQuery.CountAsync(cancellationToken);
        IReadOnlyList<Beacon> beacons = await beaconQuery
            .OrderBy(beacon => beacon.Code)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (beacons, totalCount);
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
