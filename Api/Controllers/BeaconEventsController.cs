using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaliseManager.Api.Controllers;

[ApiController]
[Route("api/beacon-events")]
public sealed class BeaconEventsController(
    IBeaconEventService beaconEventService)
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
}
