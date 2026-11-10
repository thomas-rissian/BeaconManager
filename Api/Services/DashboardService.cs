using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services.Interfaces;

namespace BaliseManager.Api.Services;

public sealed class DashboardService(IDashboardRepository dashboardRepository)
    : IDashboardService
{
    private const int RecentEventsLimit = 10;

    public async Task<DashboardSummaryResponse> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Beacon> beacons =
            await dashboardRepository.GetBeaconsAsync(cancellationToken);
        int unreadEvents =
            await dashboardRepository.GetUnreadEventCountAsync(
                cancellationToken);

        return new DashboardSummaryResponse(
            beacons.Count,
            beacons.Count(beacon => beacon.Status == BeaconStatus.Active),
            beacons.Count(beacon => beacon.Status == BeaconStatus.Inactive),
            beacons.Count(beacon => beacon.Status == BeaconStatus.Maintenance),
            beacons.Count(beacon => beacon.Status == BeaconStatus.Unknown),
            unreadEvents);
    }

    public async Task<IReadOnlyList<BeaconEventResponse>> GetRecentEventsAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<BeaconEvent> events =
            await dashboardRepository.GetRecentEventsAsync(
                RecentEventsLimit,
                cancellationToken);

        return events
            .Select(MapEventToResponse)
            .ToList();
    }

    public async Task<IReadOnlyList<BatteryStatusResponse>> GetBatteryStatusAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Beacon> beacons =
            await dashboardRepository.GetBeaconsAsync(cancellationToken);

        return beacons
            .Select(MapBatteryToResponse)
            .ToList();
    }

    private static BeaconEventResponse MapEventToResponse(
        BeaconEvent beaconEvent)
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

    private static BatteryStatusResponse MapBatteryToResponse(Beacon beacon)
    {
        string batteryStatus = beacon.BatteryLevel switch
        {
            null => "Unknown",
            <= 10 => "Critical",
            <= 20 => "Low",
            _ => "Normal"
        };

        return new BatteryStatusResponse(
            beacon.Id,
            beacon.Code,
            beacon.Name,
            beacon.BatteryLevel,
            batteryStatus);
    }
}
