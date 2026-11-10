using BaliseManager.Api.DTOs.Responses;

namespace BaliseManager.Api.Services.Interfaces;

public interface IBeaconService
{
    Task<IReadOnlyList<BeaconResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
