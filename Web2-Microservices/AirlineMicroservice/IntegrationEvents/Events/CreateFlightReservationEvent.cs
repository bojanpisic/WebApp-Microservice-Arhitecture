using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class CreateFlightReservationEvent : IntegrationEvent
    {
        public CreateFlightReservationEvent(int invitationId, string userId, int seatId, string passport, float price)
        {
            InvitationId = invitationId;
            UserId = userId;
            SeatId = seatId;
            Passport = passport;
            Price = price;
        }

        public int InvitationId { get; set; }

        public string UserId { get; set; }
        public int SeatId { get; set; }
        public string Passport { get; set; }
        public float Price { get; set; }
    }
}
