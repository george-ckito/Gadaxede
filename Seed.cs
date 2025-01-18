using System;
using System.Linq;
using Gadaxede.Data;
using Gadaxede.Models;

namespace Gadaxede
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            if (_context.Sensors.Any() || _context.Measurements.Any())
            {
                return;
            }

            var sensors = new[]
            {
                new Sensor { Name = "BME680", Description = "Air quality and environmental sensor" },
                new Sensor { Name = "BME280", Description = "Temperature and pressure sensor" },
                new Sensor { Name = "MQ2", Description = "Gas and smoke sensor" },
                new Sensor { Name = "SGP41", Description = "VOC and NOx gas sensor" },
            };

            _context.Sensors.AddRange(sensors);
            _context.SaveChanges();
        }
    }
}
