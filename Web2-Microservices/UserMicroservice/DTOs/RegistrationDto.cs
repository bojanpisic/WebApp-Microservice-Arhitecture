using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class RegistrationDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "You must specify password between 8 and 20 characters")]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public byte[] ImageUrl { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [RegularExpression("^[(][+][0-9]{3}[)][0-9]{2}[/][0-9]{3}[-][0-9]{3,4}", ErrorMessage = "Please enter valid phone no.")]
        public string Phone { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
