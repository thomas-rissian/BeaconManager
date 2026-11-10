using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaliseManager.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IDashboardService dashboardService)
    : ControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(DashboardSummaryResponse))]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary(
        CancellationToken cancellationToken)
    {
        DashboardSummaryResponse summary =
            await dashboardService.GetSummaryAsync(cancellationToken);

        return Ok(summary);
    }

    [HttpGet("recent-events")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(IReadOnlyList<BeaconEventResponse>))]
    public async Task<ActionResult<IReadOnlyList<BeaconEventResponse>>>
        GetRecentEvents(CancellationToken cancellationToken)
    {
        IReadOnlyList<BeaconEventResponse> events =
            await dashboardService.GetRecentEventsAsync(cancellationToken);

        return Ok(events);
    }

    [HttpGet("battery-status")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(IReadOnlyList<BatteryStatusResponse>))]
    public async Task<ActionResult<IReadOnlyList<BatteryStatusResponse>>>
        GetBatteryStatus(CancellationToken cancellationToken)
    {
        IReadOnlyList<BatteryStatusResponse> statuses =
            await dashboardService.GetBatteryStatusAsync(cancellationToken);

        return Ok(statuses);
    }
}
