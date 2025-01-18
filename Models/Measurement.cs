namespace Gadaxede.Models
{
    public class Measurement
    {
        public int Id { get; set; } 

        public Sensor Sensor { get; set; }
        public double Value { get; set; }

        public DateTime Timestamp { get; set; } 
    }
}
