using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }
        public int SeatsNumber { get; set; }
        public float PricePerDay { get; set; }
        public byte[] ImageUrl { get; set; }


        public int? RentACarServiceId { get; set; }
        public RentACarService RentACarService { get; set; }

        public int? BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<CarSpecialOffer> SpecialOffers { get; set; }
        public ICollection<CarRent> Rents { get; set; }
        public ICollection<CarRate> Rates { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Car()
        {
            Branch = null;
            RentACarService = null;
            SpecialOffers = new HashSet<CarSpecialOffer>();
            Rents = new HashSet<CarRent>();
            Rates = new HashSet<CarRate>();
        }

    }
}
