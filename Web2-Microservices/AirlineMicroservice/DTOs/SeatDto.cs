using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class SeatDto
    {
        [Required]
        public string Column { get; set; }
        [Required]
        public string Row { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public float Price { get; set; }
        public int FlightId { get; set; }
    }
}
