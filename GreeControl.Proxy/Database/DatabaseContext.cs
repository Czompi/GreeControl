using GreeNativeSdk;
using Microsoft.EntityFrameworkCore;

namespace GreeControl.Proxy.Database
{
    public class DatabaseContext : DbContext
    {

        public DatabaseContext() : base()
        {
        }
        

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AirConditioner> Devices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=devices.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirConditioner>().ToTable("Devices");
        }
    }
}
