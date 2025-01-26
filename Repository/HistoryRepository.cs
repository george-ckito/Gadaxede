using Gadaxede.Data;
using Gadaxede.Interfaces;
using Gadaxede.Models;

namespace Gadaxede.Repository
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly DataContext _context;
        public HistoryRepository(DataContext data)
        {
            _context = data;
        }
        ICollection<HistoryMeasurement> IHistoryRepository.GetMeasurements()
        {
            var mes = _context.Measurements.ToList();
            var result = new List<HistoryMeasurement>();
            foreach (var m in mes)
            {
                var sensor = _context.Sensors.FirstOrDefault(s => s.Id == m.SensorId);
                result.Add(new HistoryMeasurement
                {
                    SensorName = sensor.Name,
                    Value = m.Value,
                    Timestamp = m.Timestamp
                });
            }
            return result;
        }

        ICollection<HistoryMeasurement> IHistoryRepository.GetMeasurementsBySensorId(int sensorId)
        {
            throw new NotImplementedException();
        }
        ICollection<HistoryMeasurement> IHistoryRepository.GetMinuteMeasurements()
        {
            DateTime now = DateTime.Now;
            var mes = _context.Measurements
                .Where(m => m.Timestamp >= now.AddMinutes(-1))
                .ToList();
            var result = new List<HistoryMeasurement>();
            foreach (var m in mes)
            {
                var sensor = _context.Sensors.FirstOrDefault(s => s.Id == m.SensorId);
                result.Add(new HistoryMeasurement
                {
                    SensorName = sensor.Name,
                    Value = m.Value,
                    Timestamp = m.Timestamp
                });
            }
            return result;
        }
        ICollection<HistoryMeasurement> IHistoryRepository.GetHourMeasurements()
        {
            DateTime now = DateTime.Now;
            var mes = _context.Measurements
                .Where(m => m.Timestamp >= now.AddHours(-1))
                .ToList();
            var result = new List<HistoryMeasurement>();
            foreach (var m in mes)
            {
                var sensor = _context.Sensors.FirstOrDefault(s => s.Id == m.SensorId);
                result.Add(new HistoryMeasurement
                {
                    SensorName = sensor.Name,
                    Value = m.Value,
                    Timestamp = m.Timestamp
                });
            }
            return result;
        }
        ICollection<HistoryMeasurement> IHistoryRepository.GetDayMeasurements()
        {
            DateTime now = DateTime.Now;
            var mes = _context.Measurements
                .Where(m => m.Timestamp >= now.AddDays(-1))
                .ToList();
            var result = new List<HistoryMeasurement>();
            foreach (var m in mes)
            {
                var sensor = _context.Sensors.FirstOrDefault(s => s.Id == m.SensorId);
                result.Add(new HistoryMeasurement
                {
                    SensorName = sensor.Name,
                    Value = m.Value,
                    Timestamp = m.Timestamp
                });
            }
            return result;
        }
        ICollection<HistoryMeasurement> IHistoryRepository.GetWeekMeasurements()
        {
            DateTime now = DateTime.Now;
            var mes = _context.Measurements
                .Where(m => m.Timestamp >= now.AddDays(-7))
                .ToList();
            var result = new List<HistoryMeasurement>();
            foreach (var m in mes)
            {
                var sensor = _context.Sensors.FirstOrDefault(s => s.Id == m.SensorId);
                result.Add(new HistoryMeasurement
                {
                    SensorName = sensor.Name,
                    Value = m.Value,
                    Timestamp = m.Timestamp
                });
            }
            return result;
        }
    }
}
