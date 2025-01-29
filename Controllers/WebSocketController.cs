using Gadaxede.Interfaces;
using Gadaxede.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[Route("api/v1/client")]
[ApiController]
public class WebSocketController : Controller
{
    private readonly IWebSocketRepository _webSocketRepository;
    private readonly ISensorRepository _sensorRepository;

    public WebSocketController(IWebSocketRepository webSocketRepository, ISensorRepository sensorRepository)
    {
        _webSocketRepository = webSocketRepository;
        _sensorRepository = sensorRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ConnectAsync()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var socketFinishedTcs = new TaskCompletionSource<object>();
            await _webSocketRepository.HandleWebSocketAsync(webSocket, socketFinishedTcs);
            return new EmptyResult();
        }
        else
        {
            return BadRequest("This endpoint requires a WebSocket request.");
        }
    }

    [HttpPost("signal")]
    public IActionResult SaveSignal([FromBody] Dictionary<string, int> body)
    {
        try
        {
            foreach (var e in body)
            {
                var sensor = _sensorRepository.GetSensor(e.Key);
                if (sensor != null)
                {
                    _webSocketRepository.SaveSignalData(sensor, e.Value);
                }
                else
                {
                    return NotFound($"Sensor with name '{e.Key}' not found.");
                }
            }
            return Ok();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("signal")]
    public IActionResult GetSignal([FromQuery] string sensor)
    {
        try
        {
            var sensorObj = _sensorRepository.GetSensor(sensor);
            if (sensorObj != null)
            {
                var signalData = _webSocketRepository.GetSignalData(sensorObj);
                return Ok(signalData);
            }
            else
            {
                return NotFound($"Sensor with name '{sensor}' not found.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "Internal server error");
        }
    }
}