using AirlineMicroservice.DTOs;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class SendMailIntegrationEvent: IntegrationEvent
    {
        public SendMailIntegrationEvent(string userId, TripInfo tripInfo)
        {
            UserId = userId;
            TripInfo = tripInfo;
        }

        public string UserId { get; set; }
        public TripInfo TripInfo { get; set; }

        
    }
}
