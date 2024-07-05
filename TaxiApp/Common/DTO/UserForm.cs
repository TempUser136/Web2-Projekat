using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    [DataContract]
    public class UserForm
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
        public string ImagePath { get; set; } // Store the path of the image

        public UserForm(UserFormModel ufm) { 
            this.Username  = ufm.Username;
            this.Email = ufm.Email;
            this.Password = ufm.Password;
            this.Name = ufm.Name;
            this.LastName = ufm.LastName;
            this.Birthday = ufm.Birthday;
            this.Address = ufm.Address;
            this.Type = ufm.Type;
        }
    }
}
