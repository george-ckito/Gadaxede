using Gadaxede.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
                .WithMany()
                .HasForeignKey(m => m.SensorId);
            modelBuilder.Entity<Signal>()
                .HasOne(m => m.Sensor)
                .WithMany()
                .HasForeignKey(m => m.SensorId);
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            modelBuilder.Entity<Measurement>().Property(e => e.Timestamp).HasConversion(dateTimeConverter);
        }
    }
}
