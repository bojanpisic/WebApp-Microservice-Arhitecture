using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class SpecialOfferDto
    {
        [Required]
        public float NewPrice { get; set; }
        [Required]
        public List<int> SeatsIds { get; set; }
    }
}
