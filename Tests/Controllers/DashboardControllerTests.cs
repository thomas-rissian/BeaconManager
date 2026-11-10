using BaliseManager.Api.Controllers;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BaliseManager.Tests.Controllers;

public sealed class DashboardControllerTests
{
    [Fact]
    public async Task GetSummaryAsync_ReturnsOkWithSummary()
    {
        DashboardSummaryResponse expectedSummary =
            new DashboardSummaryResponse(7, 5, 1, 1, 0, 3);
        Mock<IDashboardService> serviceMock =
            new Mock<IDashboardService>();
        serviceMock
            .Setup(service => service.GetSummaryAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSummary);
        DashboardController controller =
            new DashboardController(serviceMock.Object);

        ActionResult<DashboardSummaryResponse> result =
            await controller.GetSummary(CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedSummary, okResult.Value);
    }

    [Fact]
    public async Task GetRecentEventsAsync_ReturnsOkWithEvents()
    {
        IReadOnlyList<BeaconEventResponse> expectedEvents =
        [
            new BeaconEventResponse(
                1,
                10,
                "LowBattery",
                "Battery is low.",
                DateTime.UtcNow,
                15,
                false)
        ];
        Mock<IDashboardService> serviceMock =
            new Mock<IDashboardService>();
        serviceMock
            .Setup(service => service.GetRecentEventsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvents);
        DashboardController controller =
            new DashboardController(serviceMock.Object);

        ActionResult<IReadOnlyList<BeaconEventResponse>> result =
            await controller.GetRecentEvents(CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedEvents, okResult.Value);
    }

    [Fact]
    public async Task GetBatteryStatusAsync_ReturnsOkWithStatuses()
    {
        IReadOnlyList<BatteryStatusResponse> expectedStatuses =
        [
            new BatteryStatusResponse(
                1,
                "BCN-001",
                "Workshop Beacon",
                96,
                "Normal")
        ];
        Mock<IDashboardService> serviceMock =
            new Mock<IDashboardService>();
        serviceMock
            .Setup(service => service.GetBatteryStatusAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedStatuses);
        DashboardController controller =
            new DashboardController(serviceMock.Object);

        ActionResult<IReadOnlyList<BatteryStatusResponse>> result =
            await controller.GetBatteryStatus(CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedStatuses, okResult.Value);
    }
}
