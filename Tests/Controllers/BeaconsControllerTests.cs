using BaliseManager.Api.Controllers;
using BaliseManager.Api.DTOs.Requests;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BaliseManager.Tests.Controllers;

public sealed class BeaconsControllerTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsOkWithBeaconsAndForwardsCancellationToken()
    {
        IReadOnlyList<BeaconResponse> expectedBeacons =
        [
            new BeaconResponse
            {
                Id = 1,
                Code = "BCN-001",
                Name = "Workshop Beacon",
                Status = "Active"
            }
        ];
        CancellationToken cancellationToken = new CancellationTokenSource().Token;
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedBeacons);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<IReadOnlyList<BeaconResponse>> result =
            await controller.GetAll(cancellationToken);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedBeacons, okResult.Value);
        serviceMock.Verify(
            service => service.GetAllAsync(cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBeaconExists_ReturnsOkWithBeacon()
    {
        BeaconResponse expectedBeacon = new BeaconResponse
        {
            Id = 1,
            Code = "BCN-001",
            Name = "Workshop Beacon",
            Status = "Active"
        };
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBeacon);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.GetById(1, CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedBeacon, okResult.Value);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBeaconDoesNotExist_ReturnsNotFound()
    {
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.GetByIdAsync(
                999999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeaconResponse?)null);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.GetById(999999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenBeaconIsCreated_ReturnsCreatedAtAction()
    {
        CreateBeaconRequest request = new CreateBeaconRequest
        {
            Code = "BCN-NEW",
            Name = "New Beacon",
            Status = Api.Models.BeaconStatus.Active
        };
        BeaconResponse createdBeacon = new BeaconResponse
        {
            Id = 10,
            Code = "BCN-NEW",
            Name = "New Beacon",
            Status = "Active"
        };
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBeacon);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.Create(request, CancellationToken.None);

        CreatedAtActionResult createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BeaconsController.GetById), createdResult.ActionName);
        Assert.Equal(10, createdResult.RouteValues?["id"]);
        Assert.Same(createdBeacon, createdResult.Value);
    }

    [Fact]
    public async Task CreateAsync_WhenCodeAlreadyExists_ReturnsConflict()
    {
        CreateBeaconRequest request = new CreateBeaconRequest
        {
            Code = "BCN-001",
            Name = "Duplicate Beacon"
        };
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeaconResponse?)null);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.Create(request, CancellationToken.None);

        ConflictObjectResult conflictResult =
            Assert.IsType<ConflictObjectResult>(result.Result);
        ProblemDetails problemDetails =
            Assert.IsType<ProblemDetails>(conflictResult.Value);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        Assert.Equal("Beacon code already exists.", problemDetails.Title);
    }

    [Fact]
    public async Task UpdateAsync_WhenBeaconIsUpdated_ReturnsOkWithBeacon()
    {
        UpdateBeaconRequest request = new UpdateBeaconRequest
        {
            Code = "BCN-001",
            Name = "Updated Beacon"
        };
        BeaconResponse updatedBeacon = new BeaconResponse
        {
            Id = 1,
            Code = "BCN-001",
            Name = "Updated Beacon",
            Status = "Active"
        };
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.UpdateAsync(
                1,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((updatedBeacon, false));
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.Update(1, request, CancellationToken.None);

        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updatedBeacon, okResult.Value);
    }

    [Fact]
    public async Task UpdateAsync_WhenBeaconDoesNotExist_ReturnsNotFound()
    {
        UpdateBeaconRequest request = new UpdateBeaconRequest
        {
            Code = "BCN-MISSING",
            Name = "Missing Beacon"
        };
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.UpdateAsync(
                999999,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((BeaconResponse?)null, false));
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.Update(999999, request, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAsync_WhenCodeBelongsToAnotherBeacon_ReturnsConflict()
    {
        UpdateBeaconRequest request = new UpdateBeaconRequest
        {
            Code = "BCN-002",
            Name = "Conflicting Beacon"
        };
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.UpdateAsync(
                1,
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(((BeaconResponse?)null, true));
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        ActionResult<BeaconResponse> result =
            await controller.Update(1, request, CancellationToken.None);

        ConflictObjectResult conflictResult =
            Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
    }

    [Fact]
    public async Task DeactivateAsync_WhenBeaconExists_ReturnsNoContent()
    {
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.DeactivateAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        IActionResult result =
            await controller.Deactivate(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeactivateAsync_WhenBeaconDoesNotExist_ReturnsNotFound()
    {
        Mock<IBeaconService> serviceMock = new Mock<IBeaconService>();
        serviceMock
            .Setup(service => service.DeactivateAsync(
                999999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        BeaconsController controller = new BeaconsController(serviceMock.Object);

        IActionResult result =
            await controller.Deactivate(999999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
