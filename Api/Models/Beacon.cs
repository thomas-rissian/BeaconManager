using System.ComponentModel.DataAnnotations;

namespace BaliseManager.Api.Models;

public class Beacon
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Equipment { get; set; }

    [MaxLength(120)]
    public string? Site { get; set; }

    public BeaconStatus Status { get; set; } = BeaconStatus.Unknown;

    [Range(0, 100)]
    public int? BatteryLevel { get; set; }

    public DateTime? LastCommunicationUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<BeaconEvent> Events { get; set; }
        = new List<BeaconEvent>();
}