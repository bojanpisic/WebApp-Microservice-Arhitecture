using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Models
{
    public class Address
    {
        public Address()
        {

        }
        public int AddressId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int RentACarServiceId { get; set; }
        public RentACarService RentACarService { get; set; }
    }
}
