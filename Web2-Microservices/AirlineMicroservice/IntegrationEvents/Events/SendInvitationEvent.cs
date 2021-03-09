using AirlineMicroservice.DTOs;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class SendInvitationEvent:IntegrationEvent
    {
        public SendInvitationEvent(List<InvitationInfo> invitationInfo)
        {
            InvitationInfo = invitationInfo;
        }

        public List<InvitationInfo> InvitationInfo { get; set; }
    }
}
