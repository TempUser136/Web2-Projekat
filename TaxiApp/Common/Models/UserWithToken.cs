using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public  class UserWithToken
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public DateTime Birthday { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public byte[] Image { get; set; } // Store the path of the image
        public string Token { get; set; }
    }
}
