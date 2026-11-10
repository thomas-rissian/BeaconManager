using System.ComponentModel.DataAnnotations;
using BaliseManager.Api.Models;

namespace BaliseManager.Api.DTOs.Requests;

/// <summary>
/// Defines the data required to simulate a beacon event.
/// </summary>
public sealed record SimulateBeaconEventRequest
{
    [Required]
    public BeaconEventType EventType { get; init; }

    [Required]
    [MaxLength(500)]
    [RegularExpression(@".*\S.*")]
    public required string Message { get; init; }

    [Range(0, 100)]
    public int? BatteryLevel { get; init; }
}
