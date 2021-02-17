using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Models
{
    public class RentCarServiceRates
    {
        public int RentCarServiceRatesId { get; set; }
        public float Rate { get; set; }
        public RentACarService RentACarService { get; set; }
        public string UserId { get; set; }
        public RentCarServiceRates()
        {

        }
    }
}
