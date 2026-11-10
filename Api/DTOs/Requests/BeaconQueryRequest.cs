using System.ComponentModel.DataAnnotations;
using BaliseManager.Api.Models;

namespace BaliseManager.Api.DTOs.Requests;

/// <summary>
/// Defines pagination, search, and filter parameters for beacon listing.
/// </summary>
public sealed record BeaconQueryRequest
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    [MaxLength(120)]
    public string? Search { get; init; }

    public BeaconStatus? Status { get; init; }

    [MaxLength(120)]
    public string? Site { get; init; }
}
