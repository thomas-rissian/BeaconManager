using BaliseManager.Api.Controllers;
using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
            new BeaconEventsController(
                serviceMock.Object,
                CreateDevelopmentEnvironment());

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
            new BeaconEventsController(
                serviceMock.Object,
                CreateDevelopmentEnvironment());

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
            new BeaconEventsController(
                serviceMock.Object,
                CreateDevelopmentEnvironment());

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
            new BeaconEventsController(
                serviceMock.Object,
                CreateDevelopmentEnvironment());

        IActionResult result =
            await controller.MarkAsRead(999999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task SimulateAsync_WhenDevelopmentAndBeaconExists_ReturnsCreated()
    {
        SimulateBeaconEventRequest request = new SimulateBeaconEventRequest
        {
            EventType = Api.Models.BeaconEventType.LowBattery,
            Message = "Simulated low battery event.",
            BatteryLevel = 12
        };
        BeaconEventResponse simulatedEvent = new BeaconEventResponse(
            12,
            10,
            "LowBattery",
            request.Message,
            DateTime.UtcNow,
            12,
            false);
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.SimulateAsync(
                10,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(simulatedEvent);
        BeaconEventsController controller =
            new BeaconEventsController(
                serviceMock.Object,
                CreateDevelopmentEnvironment());

        ActionResult<BeaconEventResponse> result =
            await controller.Simulate(10, request, CancellationToken.None);

        CreatedAtActionResult createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(
            nameof(BeaconEventsController.GetById),
            createdResult.ActionName);
        Assert.Equal(12, createdResult.RouteValues?["id"]);
        Assert.Same(simulatedEvent, createdResult.Value);
    }

    [Fact]
    public async Task SimulateAsync_WhenBeaconDoesNotExist_ReturnsNotFound()
    {
        SimulateBeaconEventRequest request = new SimulateBeaconEventRequest
        {
            EventType = Api.Models.BeaconEventType.Maintenance,
            Message = "Simulated maintenance event."
        };
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        serviceMock
            .Setup(service => service.SimulateAsync(
                999999,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeaconEventResponse?)null);
        BeaconEventsController controller =
            new BeaconEventsController(
                serviceMock.Object,
                CreateDevelopmentEnvironment());

        ActionResult<BeaconEventResponse> result =
            await controller.Simulate(999999, request, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task SimulateAsync_WhenNotDevelopment_ReturnsForbidden()
    {
        SimulateBeaconEventRequest request = new SimulateBeaconEventRequest
        {
            EventType = Api.Models.BeaconEventType.Maintenance,
            Message = "Simulation is forbidden."
        };
        Mock<IBeaconEventService> serviceMock =
            new Mock<IBeaconEventService>();
        Mock<IHostEnvironment> hostEnvironmentMock =
            new Mock<IHostEnvironment>();
        hostEnvironmentMock
            .SetupGet(environment => environment.EnvironmentName)
            .Returns("Production");
        BeaconEventsController controller =
            new BeaconEventsController(
                serviceMock.Object,
                hostEnvironmentMock.Object);

        ActionResult<BeaconEventResponse> result =
            await controller.Simulate(10, request, CancellationToken.None);

        Assert.IsType<ForbidResult>(result.Result);
        serviceMock.Verify(
            service => service.SimulateAsync(
                It.IsAny<int>(),
                It.IsAny<SimulateBeaconEventRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static IHostEnvironment CreateDevelopmentEnvironment()
    {
        Mock<IHostEnvironment> hostEnvironmentMock =
            new Mock<IHostEnvironment>();
        hostEnvironmentMock
            .SetupGet(environment => environment.EnvironmentName)
            .Returns(Environments.Development);
        return hostEnvironmentMock.Object;
    }
}
