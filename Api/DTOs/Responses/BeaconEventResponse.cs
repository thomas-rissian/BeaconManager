namespace BaliseManager.Api.DTOs.Responses;

/// <summary>
/// Represents an event returned by the API.
/// </summary>
public sealed record BeaconEventResponse(
    int Id,
    int BeaconId,
    string EventType,
    string Message,
    DateTime OccurredAtUtc,
    int? BatteryLevel,
    bool IsRead);
