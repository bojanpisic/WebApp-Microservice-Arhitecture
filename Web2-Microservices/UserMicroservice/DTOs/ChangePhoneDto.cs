using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class ChangePhoneDto
    {
        [Required]
        [RegularExpression("^[(][+][0-9]{3}[)][0-9]{2}[/][0-9]{3}[-][0-9]{3,4}", ErrorMessage = "Please enter valid phone no.")]
        public string Phone { get; set; }
    }
}
