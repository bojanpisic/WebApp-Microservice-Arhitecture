using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.IntegrationEvents.Events
{
    public class RateAirlineIntegrationEvent : IntegrationEvent
    {
        public RateAirlineIntegrationEvent(string userId, int airlineId, int rate)
        {
            UserId = userId;
            AirlineId = airlineId;
            Rate = rate;
        }

        public string UserId { get; set; }
        public int AirlineId { get; set; }
        public int Rate { get; set; }
    }
}
