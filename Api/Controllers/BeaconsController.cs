using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaliseManager.Api.Controllers;

[ApiController]
[Route("api/beacons")]
public sealed class BeaconsController(IBeaconService beaconService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(IReadOnlyList<BeaconResponse>))]
    public async Task<ActionResult<IReadOnlyList<BeaconResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var beacons = await beaconService.GetAllAsync(cancellationToken);
        return Ok(beacons);
    }
}
