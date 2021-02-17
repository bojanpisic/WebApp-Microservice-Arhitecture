using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.DTOs
{
    public class CarDto
    {
        //[Required]
        //public string ImageUrl { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int SeatsNumber { get; set; }
        [Required]
        public float PricePerDay { get; set; }
        [Required]
        public bool AddToMain { get; set; }
    }
}
