using Gadaxede.Models;
namespace Gadaxede.Interfaces
{
    public interface IHistoryRepository
    {
        ICollection<HistoryMeasurement> GetMeasurements();
        ICollection<HistoryMeasurement> GetMeasurementsBySensorId(int sensorId);
        ICollection<HistoryMeasurement> GetMinuteMeasurements();
        ICollection<HistoryMeasurement> GetHourMeasurements();
        ICollection<HistoryMeasurement> GetDayMeasurements();
        ICollection<HistoryMeasurement> GetWeekMeasurements();
    }
}
