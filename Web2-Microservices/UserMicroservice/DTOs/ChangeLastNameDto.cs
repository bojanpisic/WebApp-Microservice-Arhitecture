using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class ChangeLastNameDto
    {
        [Required]
        public string LastName { get; set; }
    }
}
