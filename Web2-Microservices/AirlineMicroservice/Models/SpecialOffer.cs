using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Models
{
    public class SpecialOffer
    {
        public int SpecialOfferId { get; set; }
        public float OldPrice { get; set; }
        public float NewPrice { get; set; }
        public bool IsReserved { get; set; }
        public Airline Airline { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public SpecialOffer()
        {
            Seats = new HashSet<Seat>();
            IsReserved = false;
        }
    }
}
