using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class UnregisteredDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public string Passport { get; set; }
        [Required]

        public int SeatId { get; set; }
    }
}
