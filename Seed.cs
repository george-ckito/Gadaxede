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
            if (_context.Users.Any() || _context.Sensors.Any() || _context.Measurements.Any())
            {
                // Database already seeded
                return;
            }

            // Seed Users
            var users = new[]
            {
                new User
                {
                    Username = "john_doe",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Password = "securepassword123",
                    PhoneNumber = "123-456-7890",
                    DiscordUsername = "john_doe#1234"
                },
                new User
                {
                    Username = "jane_smith",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Password = "securepassword456",
                    PhoneNumber = "987-654-3210",
                    DiscordUsername = "jane_smith#5678"
                }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Seed Sensors
            var sensors = new[]
            {
                new Sensor { Name = "BME680", Description = "Air quality and environmental sensor" },
                new Sensor { Name = "MQ2", Description = "Gas and smoke sensor" },
                new Sensor { Name = "SGP41", Description = "VOC and NOx gas sensor" },
                new Sensor { Name = "DHT22", Description = "Temperature and humidity sensor" },
            };

            _context.Sensors.AddRange(sensors);
            _context.SaveChanges();

            // Seed Measurements
            var measurements = new[]
            {
                new Measurement
                {
                    User = users[0],
                    Sensor = sensors[0],
                    Value = 100,
                    Timestamp = DateTime.UtcNow.AddMinutes(-30)
                },
                new Measurement
                {
                    User = users[0],
                    Sensor = sensors[1],
                    Value = 200,
                    Timestamp = DateTime.UtcNow.AddMinutes(-20)
                },
                new Measurement
                {
                    User = users[1],
                    Sensor = sensors[2],
                    Value = 300,
                    Timestamp = DateTime.UtcNow.AddMinutes(-10)
                },
                new Measurement
                {
                    User = users[1],
                    Sensor = sensors[3],
                    Value = 400,
                    Timestamp = DateTime.UtcNow
                }
            };

            _context.Measurements.AddRange(measurements);
            _context.SaveChanges();
        }
    }
}
