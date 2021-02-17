using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }
    }
}
