using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;

namespace BaliseManager.Api.Services.Interfaces;

public interface IBeaconService
{
    Task<IReadOnlyList<BeaconResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<BeaconResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<BeaconResponse?> CreateAsync(
        CreateBeaconRequest request,
        CancellationToken cancellationToken = default);

    Task<(BeaconResponse? Beacon, bool HasCodeConflict)> UpdateAsync(
        int id,
        UpdateBeaconRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default);
}
