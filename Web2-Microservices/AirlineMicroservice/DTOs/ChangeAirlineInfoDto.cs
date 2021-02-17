using AirlineMicroservice.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class ChangeAirlineInfoDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Address Address { get; set; }
        [Required]
        public string PromoDescription { get; set; }
    }
}
