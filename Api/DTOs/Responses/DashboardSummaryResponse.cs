namespace BaliseManager.Api.DTOs.Responses;

/// <summary>
/// Represents the main dashboard counters.
/// </summary>
public sealed record DashboardSummaryResponse(
    int TotalBeacons,
    int ActiveBeacons,
    int InactiveBeacons,
    int MaintenanceBeacons,
    int UnknownBeacons,
    int UnreadEvents);
