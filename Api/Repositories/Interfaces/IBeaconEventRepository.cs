using BaliseManager.Api.Models;

namespace BaliseManager.Api.Repositories.Interfaces;

public interface IBeaconEventRepository
{
    Task<IReadOnlyList<BeaconEvent>> GetByBeaconIdAsync(
        int beaconId,
        CancellationToken cancellationToken = default);

    Task<BeaconEvent?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> MarkAsReadAsync(
        int id,
        CancellationToken cancellationToken = default);
}
