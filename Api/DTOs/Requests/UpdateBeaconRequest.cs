using System.ComponentModel.DataAnnotations;
using BaliseManager.Api.Models;

namespace BaliseManager.Api.DTOs.Requests;

/// <summary>
/// Defines the data required to replace a beacon.
/// </summary>
public sealed record UpdateBeaconRequest
{
    [Required]
    [MaxLength(50)]
    [RegularExpression(@".*\S.*")]
    public required string Code { get; init; }

    [Required]
    [MaxLength(120)]
    [RegularExpression(@".*\S.*")]
    public required string Name { get; init; }

    [MaxLength(120)]
    public string? Equipment { get; init; }

    [MaxLength(120)]
    public string? Site { get; init; }

    public BeaconStatus Status { get; init; } = BeaconStatus.Unknown;

    [Range(0, 100)]
    public int? BatteryLevel { get; init; }

    public DateTime? LastCommunicationUtc { get; init; }
}
