using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Models
{
    public class Ticket2
    {
        public int Ticket2Id { get; set; }
        public int SeatId { get; set; }
        public float Price { get; set; }

        public string Passport { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public FlightReservation Reservation { get; set; }
    }
}
