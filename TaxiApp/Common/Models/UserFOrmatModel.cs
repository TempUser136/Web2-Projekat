using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Common.Models
{
    public class UserFormModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public IFormFile Image { get; set; } // Use byte array for image    }

    }
}
