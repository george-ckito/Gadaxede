namespace Gadaxede.Models
{
    public class Signal
    {
        public int Id { get; set; }
        public Sensor Sensor { get; set; }
        public int SensorId { get; set; }
        public int Value { get; set; }
    }
}
