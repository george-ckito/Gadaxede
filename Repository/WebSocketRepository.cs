using Gadaxede.Data;
using Gadaxede.Interfaces;
using Gadaxede.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

public class WebSocketRepository : IWebSocketRepository
{
    private readonly ILogger<WebSocketRepository> _logger;
    private readonly ConcurrentDictionary<string, WebSocket> _clients = new();
    private readonly DataContext _dbContext;

    public WebSocketRepository(ILogger<WebSocketRepository> logger, DataContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
    {
        var clientId = Guid.NewGuid().ToString();
        _clients[clientId] = webSocket;

        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(message, webSocket);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling WebSocket connection.");
        }
        finally
        {
            await CloseWebSocketAsync(clientId, webSocket);
            socketFinishedTcs.TrySetResult(null);
        }
    }

    private async Task HandleMessageAsync(string message, WebSocket webSocket)
    {
        try
        {
            var sensorData = JsonConvert.DeserializeObject<Dictionary<string, double>>(message);
            if (sensorData != null)
            {
                foreach (var sens in sensorData)
                {
                    var sensor = _dbContext.Sensors.FirstOrDefault(s => s.Name == sens.Key);
                    if (sensor != null)
                    {
                        await SaveRecordToDatabaseAsync(sensor, sens.Value);
                    }
                }
            }
            await SendToAllClientsAsync(message);
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Invalid JSON received: {ex.Message}");
        }
    }

    private async Task SendToAllClientsAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var client in _clients.Values)
        {
            if (client.State == WebSocketState.Open)
            {
                try
                {
                    await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending message to WebSocket client.");
                }
            }
        }
    }

    private async Task SaveRecordToDatabaseAsync(Sensor sensor, double value)
    {
        try
        {
            _dbContext.Measurements.Add(new Measurement
            {
                Sensor = sensor,
                Value = value,
                Timestamp = DateTime.Now
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving measurement to database.");
        }
    }

    private async Task CloseWebSocketAsync(string clientId, WebSocket webSocket)
    {
        if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        _clients.TryRemove(clientId, out _);
    }
}
