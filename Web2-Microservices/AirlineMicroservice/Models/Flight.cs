using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Models
{
    public class Flight
    {
        public int FlightId { get; set; }
        public int AirlineId { get; set; }
        public Airline Airline { get; set; }
        public string FlightNumber { get; set; }
        public System.DateTime TakeOffDateTime { get; set; }
        public System.DateTime LandingDateTime { get; set; }
        public string TripTime { get; set; }
        public float tripLength { get; set; }
        //public string TakeOffTime { get; set; }
        //public string LandingTime { get; set; }
        public ICollection<FlightDestination> Stops { get; set; }
        public Destination From { get; set; }
        public Destination To { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<FlightRate> Rates { get; set; }

        public Flight()
        {
            Rates = new HashSet<FlightRate>();
            Seats = new HashSet<Seat>();
            Stops = new HashSet<FlightDestination>();
        }
    }
}
