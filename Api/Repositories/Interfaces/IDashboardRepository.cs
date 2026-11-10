using BaliseManager.Api.Models;

namespace BaliseManager.Api.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<IReadOnlyList<Beacon>> GetBeaconsAsync(
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadEventCountAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BeaconEvent>> GetRecentEventsAsync(
        int limit,
        CancellationToken cancellationToken = default);
}
