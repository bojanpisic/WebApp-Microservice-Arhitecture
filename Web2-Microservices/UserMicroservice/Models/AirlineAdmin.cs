using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class AirlineAdmin : Person
    {
        public int AirlineId { get; set; }
        public AirlineAdmin()
        {
        }
    }
}
