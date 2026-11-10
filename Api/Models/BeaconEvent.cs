using System.ComponentModel.DataAnnotations;

namespace BaliseManager.Api.Models;

public class BeaconEvent
{
    public int Id { get; set; }

    public int BeaconId { get; set; }

    public BeaconEventType EventType { get; set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

    [Range(0, 100)]
    public int? BatteryLevel { get; set; }

    public bool IsRead { get; set; } = false;

    public Beacon Beacon { get; set; } = null!;
}