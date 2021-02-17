using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Models
{
    public class FlightDestination
    {
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        public int DestinationId { get; set; }
        public Destination Destination { get; set; }
    }
}
