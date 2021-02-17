using AirlineMicroservice.DTOs;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class SendMailToFriendIntegrationEvent: IntegrationEvent
    {
        public SendMailToFriendIntegrationEvent(string inviterId, string receiverId, FlightInfo flightInfo)
        {
            InviterId = inviterId;
            ReceiverId = receiverId;
            FlightInfo = flightInfo;
        }

        public string InviterId { get; set; }
        public string ReceiverId { get; set; }
        public FlightInfo FlightInfo { get; set; }
    }
}
