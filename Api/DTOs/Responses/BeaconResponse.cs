namespace BaliseManager.Api.DTOs.Responses;

public sealed class BeaconResponse
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Equipment { get; init; }

    public string? Site { get; init; }

    public string Status { get; init; } = string.Empty;

    public int? BatteryLevel { get; init; }

    public DateTime? LastCommunicationUtc { get; init; }
}
