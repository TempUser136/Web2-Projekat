﻿using Microsoft.EntityFrameworkCore;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Common.DTO;

namespace API.Infrastructure
{
    public class FacultyDbContext : DbContext
    {
        public DbSet<UserDto> Users { get; set; }
        public FacultyDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify composite primary key using Fluent API
            modelBuilder.Entity<UserDto>()
                .HasKey(ur => new { ur.Username});

            // Configure other properties and relationships if necessary
        }

    }
    
    public class YourDbCOntextFactory : IDesignTimeDbContextFactory<FacultyDbContext>
    {
        public FacultyDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FacultyDbContext>();
            optionsBuilder.UseOracle("User Id=sys;Password=123;Data Source=localhost:1521/xepdb1;DBA Privilege=SYSDBA;");
            return new FacultyDbContext(optionsBuilder.Options);
        }
    }
}
