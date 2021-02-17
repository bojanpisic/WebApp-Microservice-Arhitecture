using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Models
{
    public class CarSpecialOffer
    {
        public int CarSpecialOfferId { get; set; }
        public float OldPrice { get; set; }
        public float NewPrice { get; set; }
        public bool IsReserved { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public Car Car { get; set; }
        public CarSpecialOffer()
        {
            IsReserved = false;
        }
    }
}
