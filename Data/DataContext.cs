using Gadaxede.Models;
using Microsoft.EntityFrameworkCore;

namespace Gadaxede.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Signal> Signals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Measurement>()
                .HasOne(m => m.Sensor)
                .WithMany() // Measurements is not directly navigable from Sensor
                .HasForeignKey("SensorId");
            modelBuilder.Entity<Signal>()
                .HasOne(m => m.Sensor)
                .WithMany() // Measurements is not directly navigable from User
                .HasForeignKey("SensorId");
        }
    }
}
