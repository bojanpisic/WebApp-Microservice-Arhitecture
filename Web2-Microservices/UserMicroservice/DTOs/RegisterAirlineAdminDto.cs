using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class RegisterAirlineAdminDto:RegisterAdminDto
    {
        //Airline info
        [Required]
        public string Name { get; set; }
        [Required]

        public string City { get; set; }
        [Required]

        public string State { get; set; }
        [Required]

        public double Lat { get; set; }
        [Required]

        public double Lon { get; set; }
    }
}
