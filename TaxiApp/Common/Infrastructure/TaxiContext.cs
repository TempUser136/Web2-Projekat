using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class TaxiContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public TaxiContext(DbContextOptions options) : base(options)
        {
                
        }
    }
}
