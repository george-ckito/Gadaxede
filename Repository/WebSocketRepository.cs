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
    static private readonly List<WebSocket> _clients = new();
    private readonly DataContext _dbContext;

    public WebSocketRepository(ILogger<WebSocketRepository> logger, DataContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
    {
        _clients.Add(webSocket);
        _logger.LogInformation("Client connected");
        _logger.LogInformation($"Client count: {_clients.Count}");
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
            await CloseWebSocketAsync(webSocket);
            _logger.LogInformation("Client disconnected");
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
                        Console.WriteLine("Pretend this is saving data to db");
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

        foreach (var websocket in _clients)
        {
            _logger.LogInformation("Processing client with state: {State}", websocket.State);

            if (websocket.State == WebSocketState.Open)
            {
                try
                {
                    var segment = new ArraySegment<byte>(buffer);
                    await websocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    _logger.LogInformation("Message sent to client with ID");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending message to WebSocket client");
                }
            }
            else
            {
                _logger.LogWarning("Processing client with state: {State}", websocket.State);
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

    private async Task CloseWebSocketAsync(WebSocket webSocket)
    {
        if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        Console.WriteLine("WebSocket closed");
        _clients.Remove(webSocket);
    }
    public void SaveSignalData(Sensor sensor, int value)
    {
        var model = _dbContext.Signals.FirstOrDefault(m => m.Sensor == sensor);
        if (model == null)
        {
            _dbContext.Signals.Add(new Signal { Sensor = sensor, Value = value });
        }
        else
        {
            model.Value = value;
        }
        _dbContext.SaveChanges();
    }

    public Signal? GetSignalData(Sensor sensor)
    {
        return _dbContext.Signals.FirstOrDefault(m => m.Sensor == sensor);
    }
}