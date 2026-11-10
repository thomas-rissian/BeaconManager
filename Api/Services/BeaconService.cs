using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services.Interfaces;

namespace BaliseManager.Api.Services;

public sealed class BeaconService(IBeaconRepository beaconRepository)
    : IBeaconService
{
    public async Task<IReadOnlyList<BeaconResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var beacons = await beaconRepository.GetAllAsync(cancellationToken);

        return beacons
            .Select(beacon => new BeaconResponse
            {
                Id = beacon.Id,
                Code = beacon.Code,
                Name = beacon.Name,
                Equipment = beacon.Equipment,
                Site = beacon.Site,
                Status = beacon.Status.ToString(),
                BatteryLevel = beacon.BatteryLevel,
                LastCommunicationUtc = beacon.LastCommunicationUtc
            })
            .ToList();
    }
}
