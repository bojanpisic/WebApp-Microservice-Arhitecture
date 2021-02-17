using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class PassengerDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public int SeatId { get; set; }
    }
}
