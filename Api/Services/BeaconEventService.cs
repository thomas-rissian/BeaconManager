using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services.Interfaces;

namespace BaliseManager.Api.Services;

public sealed class BeaconEventService(
    IBeaconEventRepository beaconEventRepository)
    : IBeaconEventService
{
    public async Task<IReadOnlyList<BeaconEventResponse>> GetByBeaconIdAsync(
        int beaconId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<BeaconEvent> events =
            await beaconEventRepository.GetByBeaconIdAsync(
                beaconId,
                cancellationToken);

        return events
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<BeaconEventResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        BeaconEvent? beaconEvent =
            await beaconEventRepository.GetByIdAsync(id, cancellationToken);

        return beaconEvent is null ? null : MapToResponse(beaconEvent);
    }

    public Task<bool> MarkAsReadAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return beaconEventRepository.MarkAsReadAsync(id, cancellationToken);
    }

    private static BeaconEventResponse MapToResponse(BeaconEvent beaconEvent)
    {
        return new BeaconEventResponse(
            beaconEvent.Id,
            beaconEvent.BeaconId,
            beaconEvent.EventType.ToString(),
            beaconEvent.Message,
            beaconEvent.OccurredAtUtc,
            beaconEvent.BatteryLevel,
            beaconEvent.IsRead);
    }
}
