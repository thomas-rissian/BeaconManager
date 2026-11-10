using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using BaliseManager.Api.DTOs.Responses;

namespace BaliseManager.Api.WebSockets;

/// <summary>
/// Manages WebSocket connections and broadcasts beacon events.
/// </summary>
public sealed class BeaconEventWebSocketManager(
    ILogger<BeaconEventWebSocketManager> logger) : IBeaconEventNotifier
{
    private readonly ConcurrentDictionary<Guid, WebSocket> connections = new();

    // Ensures that only one broadcast or removal runs at a time.
    private readonly SemaphoreSlim broadcastLock = new(1, 1);

    private readonly JsonSerializerOptions serializerOptions = new(
        JsonSerializerDefaults.Web);

    /// <summary>
    /// Registers a connected WebSocket client.
    /// </summary>
    public Guid Add(WebSocket webSocket)
    {
        Guid connectionId = Guid.NewGuid();

        connections.TryAdd(connectionId, webSocket);

        logger.LogInformation(
            "WebSocket client {ConnectionId} connected.",
            connectionId);

        return connectionId;
    }

    /// <summary>
    /// Removes a client from the connection registry.
    /// The middleware remains responsible for disposing the socket.
    /// </summary>
    public async Task RemoveAsync(
        Guid connectionId,
        CancellationToken cancellationToken = default)
    {
        await broadcastLock.WaitAsync(cancellationToken);

        try
        {
            if (connections.TryRemove(connectionId, out _))
            {
                logger.LogInformation(
                    "WebSocket client {ConnectionId} removed.",
                    connectionId);
            }
        }
        finally
        {
            broadcastLock.Release();
        }
    }

    /// <summary>
    /// Sends an event to every currently connected client.
    /// Unavailable connections are removed automatically.
    /// </summary>
    public async Task BroadcastAsync(
        BeaconEventResponse beaconEvent,
        CancellationToken cancellationToken = default)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(
            beaconEvent,
            serializerOptions);

        await broadcastLock.WaitAsync(cancellationToken);

        try
        {
            foreach (var entry in connections.ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();

                WebSocket webSocket = entry.Value;

                if (webSocket.State != WebSocketState.Open)
                {
                    connections.TryRemove(entry.Key, out _);
                    continue;
                }

                try
                {
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(payload),
                        WebSocketMessageType.Text,
                        true,
                        cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    connections.TryRemove(entry.Key, out _);
                    AbortIfNeeded(webSocket);
                    throw;
                }
                catch (Exception exception) when (
                    exception is WebSocketException or
                    IOException or
                    ObjectDisposedException)
                {
                    connections.TryRemove(entry.Key, out _);
                    AbortIfNeeded(webSocket);

                    logger.LogDebug(
                        exception,
                        "WebSocket client {ConnectionId} disconnected.",
                        entry.Key);
                }
            }
        }
        finally
        {
            broadcastLock.Release();
        }
    }

    // Avoid interrupting a close handshake already initiated by the client.
    private static void AbortIfNeeded(WebSocket webSocket)
    {
        if (webSocket.State is WebSocketState.Open or
            WebSocketState.CloseSent)
        {
            webSocket.Abort();
        }
    }
}