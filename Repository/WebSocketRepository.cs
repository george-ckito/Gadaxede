using Gadaxede.Data;
using Gadaxede.Interfaces;
using Gadaxede.Models;
using MessagePack;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class WebSocketRepository : IWebSocketRepository
{
    private readonly ILogger<WebSocketRepository> _logger;
    private readonly List<WebSocket> _clients = new List<WebSocket>();
    private readonly DataContext _dbContext;

    public WebSocketRepository(ILogger<WebSocketRepository> logger, DataContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        _clients.Add(webSocket);
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                try
                {
                    var sensorData = JsonConvert.DeserializeObject<Dictionary<string, double>>(message);
                    await SendToAllClientsAsync(message);
                    foreach (var sens in sensorData)
                    {
                        var sensor = _dbContext.Sensors.FirstOrDefault(s => s.Name == sens.Key);
                        if (sensor != null)
                        {
                            await SaveRecordToDatabaseAsync(sensor, sens.Value);
                        }
                    }
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                _clients.Remove(webSocket);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }

    private async Task SendToAllClientsAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var client in _clients)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    private async Task SaveRecordToDatabaseAsync( Sensor sensor, double value)
    {
        _dbContext.Measurements.Add(new Measurement
        {
            Sensor = sensor,
            Value = value,
            Timestamp = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();
    }

    public void SaveSignalData( Sensor sensor, int value) {
        var model = _dbContext.Signals.FirstOrDefault(m =>
             m.Sensor == sensor);
        if (model == null) { 
            _dbContext.Signals.Add(new Signal { Sensor = sensor, Value = value });
            _dbContext.SaveChanges();

        }
        else
        {
            model.Value = value;
            _dbContext.SaveChanges();
        }
    }

    public Signal GetSignalData(Sensor sensor)
    {
        return _dbContext.Signals.FirstOrDefault(m =>
            m.Sensor == sensor) ?? new Signal();
    }

    
}
