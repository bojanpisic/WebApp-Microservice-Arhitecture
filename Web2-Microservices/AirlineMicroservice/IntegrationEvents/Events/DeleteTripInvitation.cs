using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class DeleteTripInvitation:IntegrationEvent
    {
        public DeleteTripInvitation(int invitationId)
        {
            InvitationId = invitationId;
        }

        public int InvitationId { get; set; }

    }
}
