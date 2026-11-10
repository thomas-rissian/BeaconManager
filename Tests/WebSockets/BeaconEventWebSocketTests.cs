using System.Net.WebSockets;
using System.Text;
using BaliseManager.Api.DTOs.Responses;
using BaliseManager.Api.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BaliseManager.Tests.WebSockets;

public sealed class BeaconEventWebSocketTests
{
    [Fact]
    public async Task Middleware_WhenRequestIsNotWebSocket_ReturnsBadRequest()
    {
        RequestDelegate next = _ =>
            throw new InvalidOperationException("The next middleware was called.");
        BeaconEventWebSocketMiddleware middleware =
            new BeaconEventWebSocketMiddleware(
                next,
                new BeaconEventWebSocketManager(
                    NullLogger<BeaconEventWebSocketManager>.Instance),
                NullLogger<BeaconEventWebSocketMiddleware>.Instance);
        DefaultHttpContext context = new DefaultHttpContext();
        context.Request.Path = "/ws/events";

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task Manager_BroadcastsTheSameJsonPayloadToAllConnections()
    {
        BeaconEventWebSocketManager manager =
            new BeaconEventWebSocketManager(
                NullLogger<BeaconEventWebSocketManager>.Instance);
        FakeWebSocket firstSocket = new FakeWebSocket();
        FakeWebSocket secondSocket = new FakeWebSocket();
        Guid firstConnection = manager.Add(firstSocket);
        Guid secondConnection = manager.Add(secondSocket);
        BeaconEventResponse beaconEvent = new BeaconEventResponse(
            1,
            10,
            "LowBattery",
            "Battery is low.",
            DateTime.UtcNow,
            12,
            false);

        await manager.BroadcastAsync(beaconEvent);

        Assert.NotEqual(firstConnection, secondConnection);
        Assert.Equal(
            Encoding.UTF8.GetString(firstSocket.LastPayload),
            Encoding.UTF8.GetString(secondSocket.LastPayload));
        Assert.Contains("\"eventType\":\"LowBattery\"", firstSocket.PayloadText);
        await manager.RemoveAsync(firstConnection);
        await manager.RemoveAsync(secondConnection);
    }

    private sealed class FakeWebSocket : WebSocket
    {
        private WebSocketState state = WebSocketState.Open;

        public byte[] LastPayload { get; private set; } = [];

        public string PayloadText => Encoding.UTF8.GetString(LastPayload);

        public override WebSocketCloseStatus? CloseStatus => null;

        public override string? CloseStatusDescription => null;

        public override WebSocketState State => state;

        public override string? SubProtocol => null;

        public override Task CloseAsync(
            WebSocketCloseStatus closeStatus,
            string? statusDescription,
            CancellationToken cancellationToken)
        {
            state = WebSocketState.Closed;
            return Task.CompletedTask;
        }

        public override Task CloseOutputAsync(
            WebSocketCloseStatus closeStatus,
            string? statusDescription,
            CancellationToken cancellationToken)
        {
            state = WebSocketState.CloseSent;
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            state = WebSocketState.Closed;
        }

        public override void Abort()
        {
            state = WebSocketState.Aborted;
        }

        public override Task<WebSocketReceiveResult> ReceiveAsync(
            ArraySegment<byte> buffer,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new WebSocketReceiveResult(
                    0,
                    WebSocketMessageType.Close,
                    true));
        }

        public override Task SendAsync(
            ArraySegment<byte> buffer,
            WebSocketMessageType messageType,
            bool endOfMessage,
            CancellationToken cancellationToken)
        {
            LastPayload = buffer.ToArray();
            return Task.CompletedTask;
        }
    }
}
