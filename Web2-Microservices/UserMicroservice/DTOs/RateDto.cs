using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class RateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Rate { get; set; }
    }
}
