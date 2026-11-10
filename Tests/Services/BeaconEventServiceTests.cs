using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Models;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services;
using BaliseManager.Api.WebSockets;
using Moq;
using Xunit;

namespace BaliseManager.Tests.Services;

public sealed class BeaconEventServiceTests
{
    [Fact]
    public async Task SimulateAsync_PersistsBeforeBroadcastingEvent()
    {
        Beacon beacon = new Beacon
        {
            Id = 10,
            Code = "BCN-010",
            Name = "Test Beacon",
            BatteryLevel = 80
        };
        SimulateBeaconEventRequest request = new SimulateBeaconEventRequest
        {
            EventType = BeaconEventType.LowBattery,
            Message = "  Battery is low.  ",
            BatteryLevel = 12
        };
        Mock<IBeaconRepository> beaconRepositoryMock =
            new Mock<IBeaconRepository>();
        Mock<IBeaconEventRepository> eventRepositoryMock =
            new Mock<IBeaconEventRepository>();
        Mock<IBeaconEventNotifier> notifierMock =
            new Mock<IBeaconEventNotifier>();
        MockSequence sequence = new MockSequence();

        beaconRepositoryMock
            .InSequence(sequence)
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(beacon);
        eventRepositoryMock
            .InSequence(sequence)
            .Setup(repository => repository.AddAsync(
                It.IsAny<BeaconEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        beaconRepositoryMock
            .InSequence(sequence)
            .Setup(repository => repository.UpdateAsync(
                beacon,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        notifierMock
            .InSequence(sequence)
            .Setup(notifier => notifier.BroadcastAsync(
                It.IsAny<BeaconEventResponse>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        BeaconEventService service = new BeaconEventService(
            eventRepositoryMock.Object,
            beaconRepositoryMock.Object,
            notifierMock.Object);

        BeaconEventResponse? response = await service.SimulateAsync(
            10,
            request,
            CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal("Battery is low.", response.Message);
        Assert.Equal(12, response.BatteryLevel);
        Assert.Equal(12, beacon.BatteryLevel);
        notifierMock.Verify(
            notifier => notifier.BroadcastAsync(
                It.Is<BeaconEventResponse>(eventResponse =>
                    eventResponse.BeaconId == 10 &&
                    eventResponse.EventType == "LowBattery"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SimulateAsync_WhenBeaconDoesNotExist_DoesNotBroadcast()
    {
        Mock<IBeaconRepository> beaconRepositoryMock =
            new Mock<IBeaconRepository>();
        Mock<IBeaconEventRepository> eventRepositoryMock =
            new Mock<IBeaconEventRepository>();
        Mock<IBeaconEventNotifier> notifierMock =
            new Mock<IBeaconEventNotifier>();
        beaconRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                999999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beacon?)null);
        BeaconEventService service = new BeaconEventService(
            eventRepositoryMock.Object,
            beaconRepositoryMock.Object,
            notifierMock.Object);

        BeaconEventResponse? response = await service.SimulateAsync(
            999999,
            new SimulateBeaconEventRequest
            {
                EventType = BeaconEventType.Maintenance,
                Message = "Maintenance"
            },
            CancellationToken.None);

        Assert.Null(response);
        notifierMock.Verify(
            notifier => notifier.BroadcastAsync(
                It.IsAny<BeaconEventResponse>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
