
using BaliseManager.Api.DTOs.Responses;

namespace BaliseManager.Api.WebSockets;

/// <summary>
/// Defines the contract for broadcasting beacon events to connected clients.
/// </summary>
public interface IBeaconEventNotifier
{
    /// <summary>
    /// Broadcasts a beacon event to all currently connected clients.
    /// </summary>
    /// <param name="beaconEvent">The event to broadcast.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task BroadcastAsync(
        BeaconEventResponse beaconEvent,
        CancellationToken cancellationToken = default);
}
