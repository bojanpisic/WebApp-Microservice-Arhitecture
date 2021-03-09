using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class RejectTripInvitationEvent : IntegrationEvent
    {
        public int SeatId { get; set; }
        public int InvitationId { get; set; }


        public RejectTripInvitationEvent(int seatId, int invitationId)
        {
            SeatId = seatId;
            InvitationId = invitationId;
        }
    }
}
