using System.IO;
using System.Net.WebSockets;

namespace BaliseManager.Api.WebSockets;

/// <summary>
/// Handles WebSocket connections on the /ws/events endpoint.
/// </summary>
public sealed class BeaconEventWebSocketMiddleware(
    RequestDelegate next,
    BeaconEventWebSocketManager webSocketManager,
    ILogger<BeaconEventWebSocketMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.Equals(
                "/ws/events",
                StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        // The middleware owns and disposes the WebSocket.
        using WebSocket webSocket =
            await context.WebSockets.AcceptWebSocketAsync();

        Guid connectionId = webSocketManager.Add(webSocket);

        try
        {
            await ReceiveUntilClosedAsync(
                webSocket,
                context.RequestAborted);

            // Remove the connection before completing the close handshake.
            // This prevents a new broadcast from sending to this socket.
            await webSocketManager.RemoveAsync(connectionId);

            if (webSocket.State == WebSocketState.CloseReceived)
            {
                await webSocket.CloseOutputAsync(
                    webSocket.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    webSocket.CloseStatusDescription,
                    CancellationToken.None);
            }
        }
        catch (OperationCanceledException) when (
            context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug(
                "WebSocket client {ConnectionId} request was cancelled.",
                connectionId);

            webSocket.Abort();
        }
        catch (Exception exception) when (
            exception is WebSocketException or
            IOException or
            ObjectDisposedException)
        {
            logger.LogDebug(
                exception,
                "WebSocket client {ConnectionId} disconnected unexpectedly.",
                connectionId);

            webSocket.Abort();
        }
        finally
        {
            await webSocketManager.RemoveAsync(
                connectionId,
                CancellationToken.None);
        }
    }

    /// <summary>
    /// Keeps receiving until the client sends a close frame or disconnects.
    /// Application messages are ignored.
    /// </summary>
    private static async Task ReceiveUntilClosedAsync(
        WebSocket webSocket,
        CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[4096];

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                return;
            }

            // Messages sent by the browser are intentionally ignored.
        }
    }
}