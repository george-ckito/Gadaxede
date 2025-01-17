using Gadaxede.Data;
using Gadaxede.Interfaces;
using Gadaxede.Models;

namespace Gadaxede.Repository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly DataContext _context;
        public SensorRepository(DataContext data)
        {
            _context = data;
        }

        public ICollection<Sensor> GetSensors() => _context.Sensors.OrderBy(p => p.Id).ToList();
        public Sensor GetSensor(int id) => _context.Sensors.Find(id);
        public Sensor GetSensor(string name) => _context.Sensors.FirstOrDefault(p => p.Name == name);
    }
}
