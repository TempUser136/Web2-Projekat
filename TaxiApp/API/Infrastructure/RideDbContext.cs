using Microsoft.EntityFrameworkCore;
using Common.Models;
using Microsoft.EntityFrameworkCore.Design;
using Common.DTO;

namespace API.Infrastructure
{
    public class RideDbContext : DbContext
    {
        public DbSet<RideDto> Rides { get; set; } // Replace Ride with your actual entity

        public RideDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify primary key using Fluent APImodelBuilder.Entity<UserDto>()
            modelBuilder.Entity<RideDto>()
                .HasKey(ur => new { ur.Id });
        }
    }

    public class RideDbContextFactory : IDesignTimeDbContextFactory<RideDbContext>
    {
        public RideDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RideDbContext>();
            optionsBuilder.UseOracle("User Id=sys;Password=123;Data Source=localhost:1521/xe;DBA Privilege=SYSDBA;");
            return new RideDbContext(optionsBuilder.Options);
        }
    }
}
