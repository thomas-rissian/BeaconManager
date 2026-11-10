using BaliseManager.Api.DTOs.Responses;

namespace BaliseManager.Api.Services.Interfaces;

public interface IBeaconEventService
{
    Task<IReadOnlyList<BeaconEventResponse>> GetByBeaconIdAsync(
        int beaconId,
        CancellationToken cancellationToken = default);

    Task<BeaconEventResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> MarkAsReadAsync(
        int id,
        CancellationToken cancellationToken = default);
}
