using BaliseManager.Api.DTOs.Requests;
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
        IReadOnlyList<BeaconResponse> beacons =
            await beaconService.GetAllAsync(cancellationToken);
        return Ok(beacons);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BeaconResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeaconResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        BeaconResponse? beacon =
            await beaconService.GetByIdAsync(id, cancellationToken);
        return beacon is null ? NotFound() : Ok(beacon);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BeaconResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BeaconResponse>> Create(
        CreateBeaconRequest request,
        CancellationToken cancellationToken)
    {
        BeaconResponse? beacon =
            await beaconService.CreateAsync(request, cancellationToken);

        if (beacon is null)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Beacon code already exists.",
                Detail = "A beacon with the same code already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = beacon.Id },
            beacon);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BeaconResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BeaconResponse>> Update(
        int id,
        UpdateBeaconRequest request,
        CancellationToken cancellationToken)
    {
        (BeaconResponse? Beacon, bool HasCodeConflict) result =
            await beaconService.UpdateAsync(id, request, cancellationToken);

        if (result.HasCodeConflict)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Beacon code already exists.",
                Detail = "A beacon with the same code already exists."
            });
        }

        return result.Beacon is null
            ? NotFound()
            : Ok(result.Beacon);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        bool deactivated =
            await beaconService.DeactivateAsync(id, cancellationToken);

        return deactivated ? NoContent() : NotFound();
    }
}
