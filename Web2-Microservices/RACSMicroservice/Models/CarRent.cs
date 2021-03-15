using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Models
{
    public class CarRent
    {
        public int CarRentId { get; set; }
        public DateTime TakeOverDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string TakeOverCity { get; set; }
        public string ReturnCity { get; set; }
        public float TotalPrice { get; set; }
        public Car RentedCar { get; set; }
        public string UserId { get; set; }
        public DateTime RentDate { get; set; }
        public int? TripReservationId { get; set; }

        public CarRent()
        {
        }
    }
}
