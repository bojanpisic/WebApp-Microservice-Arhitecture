using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class User : Person
    {
        public User()
        {
            //FlightReservations = new HashSet<FlightReservation>();
            //RateAirline = new HashSet<AirlineRate>();
            //RateRACService = new HashSet<RentCarServiceRates>();
            //CarRents = new HashSet<CarRent>();

            FriendshipRequests = new HashSet<Friendship>();
            FriendshipInvitations = new HashSet<Friendship>();
            Friends = new HashSet<User>();
            TripRequests = new HashSet<Invitation>();
            TripInvitations = new HashSet<Invitation>();
            BonusPoints = 100;
        }
        public int BonusPoints { get; set; }
        //public ICollection<FlightReservation> FlightReservations { get; set; }
        //public ICollection<CarRent> CarRents { get; set; }
        //public ICollection<AirlineRate> RateAirline { get; set; }
        //public ICollection<RentCarServiceRates> RateRACService { get; set; }
        public virtual ICollection<User> Friends { get; set; }
        public virtual ICollection<Friendship> FriendshipRequests { get; set; }
        public virtual ICollection<Friendship> FriendshipInvitations { get; set; }
        public virtual ICollection<Invitation> TripRequests { get; set; }
        public virtual ICollection<Invitation> TripInvitations { get; set; }
    }
}
