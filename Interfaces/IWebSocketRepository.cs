using Gadaxede.Models;
using System.Net.WebSockets;

namespace Gadaxede.Interfaces
{
    public interface IWebSocketRepository
    {
        Task HandleWebSocketAsync(WebSocket webSocket, User user);
        public void SaveSignalData(User user, Sensor sensor, int value);
        public Signal GetSignalData(User user, Sensor sensor);
    }
}