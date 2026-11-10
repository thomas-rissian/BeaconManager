using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services.Interfaces;
using BaliseManager.Api.WebSockets;

namespace BaliseManager.Api.Services;

public sealed class BeaconEventService(
    IBeaconEventRepository beaconEventRepository,
    IBeaconRepository beaconRepository,
    IBeaconEventNotifier beaconEventNotifier)
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

    public async Task<BeaconEventResponse?> SimulateAsync(
        int beaconId,
        SimulateBeaconEventRequest request,
        CancellationToken cancellationToken = default)
    {
        Beacon? beacon =
            await beaconRepository.GetByIdAsync(beaconId, cancellationToken);

        if (beacon is null)
        {
            return null;
        }

        DateTime occurredAtUtc = DateTime.UtcNow;
        BeaconEvent beaconEvent = new BeaconEvent
        {
            BeaconId = beaconId,
            EventType = request.EventType,
            Message = request.Message.Trim(),
            OccurredAtUtc = occurredAtUtc,
            BatteryLevel = request.BatteryLevel
        };

        if (request.BatteryLevel.HasValue)
        {
            beacon.BatteryLevel = request.BatteryLevel;
        }

        beacon.LastCommunicationUtc = occurredAtUtc;
        beacon.UpdatedAtUtc = occurredAtUtc;

        await beaconEventRepository.AddAsync(beaconEvent, cancellationToken);
        await beaconRepository.UpdateAsync(beacon, cancellationToken);

        BeaconEventResponse response = MapToResponse(beaconEvent);
        await beaconEventNotifier.BroadcastAsync(response, cancellationToken);

        return response;
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
