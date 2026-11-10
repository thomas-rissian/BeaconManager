using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.Models;

namespace BaliseManager.Api.Repositories.Interfaces;

public interface IBeaconRepository
{
    Task<(IReadOnlyList<Beacon> Beacons, int TotalCount)> GetPageAsync(
        BeaconQueryRequest query,
        CancellationToken cancellationToken = default);

    Task<Beacon?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        int? excludingId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Beacon beacon,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Beacon beacon,
        CancellationToken cancellationToken = default);
}
