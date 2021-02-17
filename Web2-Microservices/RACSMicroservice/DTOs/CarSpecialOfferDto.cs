using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.DTOs
{
    public class CarSpecialOfferDto
    {
        [Required]
        public float NewPrice { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }
}
