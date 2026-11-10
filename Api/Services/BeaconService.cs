using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services.Interfaces;

namespace BaliseManager.Api.Services;

public sealed class BeaconService(IBeaconRepository beaconRepository)
    : IBeaconService
{
    public async Task<BeaconListResponse> GetPageAsync(
        BeaconQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        (IReadOnlyList<Beacon> Beacons, int TotalCount) page =
            await beaconRepository.GetPageAsync(query, cancellationToken);

        IReadOnlyList<BeaconResponse> responses = page.Beacons
            .Select(MapToResponse)
            .ToList();

        int totalPages = (int)Math.Ceiling(
            (double)page.TotalCount / query.PageSize);

        return new BeaconListResponse(
            responses,
            query.Page,
            query.PageSize,
            page.TotalCount,
            totalPages);
    }

    public async Task<BeaconResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        Beacon? beacon =
            await beaconRepository.GetByIdAsync(id, cancellationToken);
        return beacon is null ? null : MapToResponse(beacon);
    }

    public async Task<BeaconResponse?> CreateAsync(
        CreateBeaconRequest request,
        CancellationToken cancellationToken = default)
    {
        string code = request.Code.Trim();

        if (await beaconRepository.ExistsByCodeAsync(
                code,
                cancellationToken: cancellationToken))
        {
            return null;
        }

        Beacon beacon = new Beacon
        {
            Code = code,
            Name = request.Name.Trim(),
            Equipment = request.Equipment?.Trim(),
            Site = request.Site?.Trim(),
            Status = request.Status,
            BatteryLevel = request.BatteryLevel,
            LastCommunicationUtc = request.LastCommunicationUtc
        };

        await beaconRepository.AddAsync(beacon, cancellationToken);
        return MapToResponse(beacon);
    }

    public async Task<(BeaconResponse? Beacon, bool HasCodeConflict)> UpdateAsync(
        int id,
        UpdateBeaconRequest request,
        CancellationToken cancellationToken = default)
    {
        Beacon? beacon =
            await beaconRepository.GetByIdAsync(id, cancellationToken);

        if (beacon is null)
        {
            return (null, false);
        }

        string code = request.Code.Trim();

        if (await beaconRepository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken))
        {
            return (null, true);
        }

        beacon.Code = code;
        beacon.Name = request.Name.Trim();
        beacon.Equipment = request.Equipment?.Trim();
        beacon.Site = request.Site?.Trim();
        beacon.Status = request.Status;
        beacon.BatteryLevel = request.BatteryLevel;
        beacon.LastCommunicationUtc = request.LastCommunicationUtc;
        beacon.UpdatedAtUtc = DateTime.UtcNow;

        await beaconRepository.UpdateAsync(beacon, cancellationToken);
        return (MapToResponse(beacon), false);
    }

    public async Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        Beacon? beacon =
            await beaconRepository.GetByIdAsync(id, cancellationToken);

        if (beacon is null)
        {
            return false;
        }

        beacon.Status = BeaconStatus.Inactive;
        beacon.UpdatedAtUtc = DateTime.UtcNow;
        await beaconRepository.UpdateAsync(beacon, cancellationToken);
        return true;
    }

    private static BeaconResponse MapToResponse(Beacon beacon)
    {
        return new BeaconResponse
        {
            Id = beacon.Id,
            Code = beacon.Code,
            Name = beacon.Name,
            Equipment = beacon.Equipment,
            Site = beacon.Site,
            Status = beacon.Status.ToString(),
            BatteryLevel = beacon.BatteryLevel,
            LastCommunicationUtc = beacon.LastCommunicationUtc
        };
    }
}
