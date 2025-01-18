using Gadaxede.Models;
using System.Net.WebSockets;

namespace Gadaxede.Interfaces
{
    public interface IWebSocketRepository
    {
        Task HandleWebSocketAsync(WebSocket webSocket);
        public void SaveSignalData(Sensor sensor, int value);
        public Signal GetSignalData(Sensor sensor);
    }
}