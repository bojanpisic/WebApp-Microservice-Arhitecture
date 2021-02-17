using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class FlightInfo
    {
        public string FlightNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public string TripTime { get; set; }
        public float TripLength { get; set; }
        public string SeatColumn { get; set; }
        public string SeatRow { get; set; }
        public string SeatClass { get; set; }
        public float TicketPrice { get; set; }
        public FlightInfo()
        {
        }
    }
}
