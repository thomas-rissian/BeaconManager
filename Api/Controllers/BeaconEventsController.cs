using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BaliseManager.Api.Controllers;

[ApiController]
[Route("api/beacon-events")]
public sealed class BeaconEventsController(
    IBeaconEventService beaconEventService,
    IHostEnvironment hostEnvironment)
    : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(BeaconEventResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeaconEventResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        BeaconEventResponse? beaconEvent =
            await beaconEventService.GetByIdAsync(id, cancellationToken);

        return beaconEvent is null ? NotFound() : Ok(beaconEvent);
    }

    [HttpPatch("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(
        int id,
        CancellationToken cancellationToken)
    {
        bool markedAsRead =
            await beaconEventService.MarkAsReadAsync(id, cancellationToken);

        return markedAsRead ? NoContent() : NotFound();
    }

    [HttpPost("/api/beacons/{beaconId:int}/simulate-event")]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Type = typeof(BeaconEventResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeaconEventResponse>> Simulate(
        int beaconId,
        SimulateBeaconEventRequest request,
        CancellationToken cancellationToken)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return Forbid();
        }

        BeaconEventResponse? beaconEvent =
            await beaconEventService.SimulateAsync(
                beaconId,
                request,
                cancellationToken);

        if (beaconEvent is null)
        {
            return NotFound();
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = beaconEvent.Id },
            beaconEvent);
    }
}
