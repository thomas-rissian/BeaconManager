using BaliseManager.Api.DTOs.Responses;

namespace BaliseManager.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BeaconEventResponse>> GetRecentEventsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BatteryStatusResponse>> GetBatteryStatusAsync(
        CancellationToken cancellationToken = default);
}
