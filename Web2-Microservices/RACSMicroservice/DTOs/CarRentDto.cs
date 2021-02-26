using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.DTOs
{
    public class CarRentDto
    {
        [Required]
        public int CarRentId { get; set; }
        [Required]
        public DateTime TakeOverDate { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
        [Required]
        public string TakeOverCity { get; set; }
        [Required]
        public string ReturnCity { get; set; }
    }
}
