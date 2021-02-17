using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Models
{
    public class CarReservation
    {
        public int CarRentId { get; set; }
        public DateTime TakeOverDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string TakeOverCity { get; set; }
        public string ReturnCity { get; set; }
        public float TotalPrice { get; set; }
        public int RentedCarId { get; set; }
        public string UserId { get; set; }
        public DateTime RentDate { get; set; }
        public FlightReservation FlightReservation { get; set; }

        public CarReservation()
        {
        }
    }
}
