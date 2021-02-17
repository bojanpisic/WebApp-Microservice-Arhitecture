using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
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
