namespace Gadaxede.Models
{
    public class Signal
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Sensor Sensor { get; set; }
        public int Value { get; set; }
    }
}
