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
    public async Task Connect()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var socketFinishedTcs = new TaskCompletionSource<object>();
            await _webSocketRepository.HandleWebSocketAsync(webSocket, socketFinishedTcs);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
    [HttpPost("signal")]
    public IActionResult Signal( [FromBody] Dictionary<string, int> body) {
        foreach (var e in body) {
            var sensor = _sensorRepository.GetSensor(e.Key);
            _webSocketRepository.SaveSignalData(sensor, e.Value);
        }
        return Ok();
    }
    [HttpGet("signal")]
    public IActionResult GetSignal([FromQuery] string sensor)
    {
        var sensorv = _sensorRepository.GetSensor(sensor);
        return Ok(_webSocketRepository.GetSignalData(sensorv));
    }
}
