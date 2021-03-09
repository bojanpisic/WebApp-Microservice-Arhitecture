using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class InvitationInfo
    {
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
        public int SeatId { get; set; }
        public float Price { get; set; }
        public DateTime Expires { get; set; }
        public FlightInfo FlightInfo { get; set; }
    }
}
