using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class TripInfoFriend
    {
        public FlightInfo FlightInfo { get; set; }
        public string InviterId { get; set; }

        public TripInfoFriend()
        {
        }
    }
}
