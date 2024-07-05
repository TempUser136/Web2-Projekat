namespace Common.Models
{
    public class Ride
    {
        public string StartAddress { get; set; }
        public string Destination { get; set; }
        public decimal Price { get; set; }
        public int WaitTime { get; set; }
        public string Status { get; set; }
    }
}
