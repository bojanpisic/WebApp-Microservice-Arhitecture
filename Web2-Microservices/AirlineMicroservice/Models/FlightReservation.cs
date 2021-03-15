﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Models
{
    public class FlightReservation
    {
        public int FlightReservationId { get; set; }
        public string UserId { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<Ticket2> UnregistredFriendsTickets { get; set; }
        public float Price { get; set; }
        public DateTime ReservationDate { get; set; }

        public bool IsCarRented { get; set; }

        public FlightReservation()
        {
            Tickets = new HashSet<Ticket>();
            UnregistredFriendsTickets = new HashSet<Ticket2>();
        }
    }
}
