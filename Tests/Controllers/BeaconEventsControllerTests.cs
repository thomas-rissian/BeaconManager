using BaliseManager.Api.Controllers;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BaliseManager.Tests.Controllers;

public sealed class BeaconEventsControllerTests
{
    [Fact]
    public async Task GetEventsAsync_ReturnsOkWithEvents()
    {
        IReadOnlyList<BeaconEventResponse> expectedEvents =
        [
            new BeaconEventResponse(
                1,
                10,
                "LowBattery",
                "Battery level is below the warning threshold.",
                DateTime.UtcNow,
                15,
                false)
        ];
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.GetByBeaconIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvents);
        BeaconsController controller = new BeaconsController(
            Mock.Of<IBeaconService>());

        ActionResult<IReadOnlyList<BeaconEventResponse>> result =
            await controller.GetEvents(
                10,
                serviceMock.Object,
                CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedEvents, okResult.Value);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEventExists_ReturnsOkWithEvent()
    {
        BeaconEventResponse expectedEvent = new BeaconEventResponse(
            1,
            10,
            "MotionDetected",
            "Movement detected.",
            DateTime.UtcNow,
            null,
            true);
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvent);
        BeaconEventsController controller =
            new BeaconEventsController(serviceMock.Object);

        ActionResult<BeaconEventResponse> result =
            await controller.GetById(1, CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedEvent, okResult.Value);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEventDoesNotExist_ReturnsNotFound()
    {
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.GetByIdAsync(
                999999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeaconEventResponse?)null);
        BeaconEventsController controller =
            new BeaconEventsController(serviceMock.Object);

        ActionResult<BeaconEventResponse> result =
            await controller.GetById(999999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenEventExists_ReturnsNoContent()
    {
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.MarkAsReadAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        BeaconEventsController controller =
            new BeaconEventsController(serviceMock.Object);

        IActionResult result =
            await controller.MarkAsRead(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenEventDoesNotExist_ReturnsNotFound()
    {
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.MarkAsReadAsync(
                999999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        BeaconEventsController controller =
            new BeaconEventsController(serviceMock.Object);

        IActionResult result =
            await controller.MarkAsRead(999999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
