using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.DTOs;

namespace UserMicroservice.IntegrationEvents.Events
{
    public class SendInvitationEvent : IntegrationEvent
    {
        public SendInvitationEvent(List<InvitationInfo> invitationInfo)
        {
            InvitationInfo = invitationInfo;
        }

        public List<InvitationInfo> InvitationInfo { get; set; }
    }
}
