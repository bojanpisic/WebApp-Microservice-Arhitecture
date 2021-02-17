using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Models
{
    public class AirlineRate
    {
        public int AirlineRateId { get; set; }

        public float Rate { get; set; }
        //public int AirlineId { get; set; }
        public Airline Airline { get; set; }
        public string UserId { get; set; }
        public AirlineRate()
        {
        }
    }
}
