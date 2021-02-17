using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class Person : IdentityUser
    {
        public byte[] ImageUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public bool PasswordChanged { get; set; }
        public Person()
        {
            PasswordChanged = false;
        }
    }
}
