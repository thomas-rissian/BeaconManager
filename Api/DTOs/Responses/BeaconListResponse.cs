namespace BaliseManager.Api.DTOs.Responses;

/// <summary>
/// Represents a paginated list of beacons.
/// </summary>
public sealed record BeaconListResponse(
    IReadOnlyList<BeaconResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
