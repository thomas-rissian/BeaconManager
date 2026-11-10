namespace BaliseManager.Api.DTOs.Responses;

/// <summary>
/// Represents the battery state of a beacon.
/// </summary>
public sealed record BatteryStatusResponse(
    int BeaconId,
    string Code,
    string Name,
    int? BatteryLevel,
    string BatteryStatus);
