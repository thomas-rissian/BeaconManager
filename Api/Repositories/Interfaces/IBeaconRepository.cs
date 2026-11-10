using BaliseManager.Api.Models;

namespace BaliseManager.Api.Repositories.Interfaces;

public interface IBeaconRepository
{
    Task<IReadOnlyList<Beacon>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
