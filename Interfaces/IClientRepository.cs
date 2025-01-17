using Gadaxede.Models;
namespace Gadaxede.Interfaces
{
    public interface IClientRepository
    {
        ICollection<Measurement> GetUserMeasurements(User user);
        ICollection<Measurement> GetUserSensorMeasurements(Sensor sensor);

    }
}
