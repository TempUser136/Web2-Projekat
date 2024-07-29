using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class RideDto
    {
        public int Id { get; set; } 
        public string StartAddress { get; set; }
        public string Destination { get; set; }
        public decimal Price { get; set; }
        public int WaitTime { get; set; }
        public string Status { get; set; }
    }
}
