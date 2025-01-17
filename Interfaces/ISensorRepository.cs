using Gadaxede.Models;

namespace Gadaxede.Interfaces
{
    public interface ISensorRepository
    {
        ICollection<Sensor> GetSensors();
        Sensor GetSensor(int id);
        Sensor GetSensor(string name);
    }
}
