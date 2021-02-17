using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class DestinationDto
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        public string ImgUrl { get; set; }
    }
}
