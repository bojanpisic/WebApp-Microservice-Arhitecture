using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.IntegrationEvents.Events
{
    public class FlightReservationCreatedEvent : IntegrationEvent
    {
        public FlightReservationCreatedEvent(int invitationForDeleteId, float tripLength)
        {
            InvitationForDeleteId = invitationForDeleteId;
            TripLength = tripLength;
        }

        public int InvitationForDeleteId { get; set; }
        public float TripLength { get; set; }
    }
}
