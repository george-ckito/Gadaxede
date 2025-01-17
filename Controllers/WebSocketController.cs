 using Gadaxede.Interfaces;
using Gadaxede.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[Route("api/v1/client")]
[ApiController]
public class WebSocketController : Controller
{
    private readonly IWebSocketRepository _webSocketRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISensorRepository _sensorRepository;

    public WebSocketController(IWebSocketRepository webSocketRepository, IUserRepository userRepository, ISensorRepository sensorRepository)
    {
        _webSocketRepository = webSocketRepository;
        _userRepository = userRepository;
        _sensorRepository = sensorRepository;
    }

    [HttpGet("{id}")]
    public async Task Connect(int id)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var user = _userRepository.GetUser(id);
            await _webSocketRepository.HandleWebSocketAsync(webSocket, user);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
    [HttpPost("{id}/signal")]
    public IActionResult Signal(int id, [FromBody] Dictionary<string, int> body) {
        var user = _userRepository.GetUser(id);
        foreach (var e in body) {
            var sensor = _sensorRepository.GetSensor(e.Key);
            _webSocketRepository.SaveSignalData(user, sensor, e.Value);
        }
        return Ok();
    }
    [HttpGet("{id}/signal")]
    public IActionResult GetSignal(int id, [FromQuery] string sensor)
    {
        var user = _userRepository.GetUser(id);
        var sensorv = _sensorRepository.GetSensor(sensor);
        return Ok(_webSocketRepository.GetSignalData(user, sensorv));
    }
}
