using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "You must specify password between 8 and 20 characters")]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }
        [Required]
        public string OldPassword { get; set; }
    }
}
