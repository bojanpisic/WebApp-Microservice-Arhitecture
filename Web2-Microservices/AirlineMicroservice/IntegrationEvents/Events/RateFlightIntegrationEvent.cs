using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class RateFlightIntegrationEvent: IntegrationEvent
    {
        public RateFlightIntegrationEvent(string userId, int flightId, int rate)
        {
            UserId = userId;
            FlightId = flightId;
            Rate = rate;
        }

        public string UserId { get; set; }
        public int FlightId { get; set; }
        public int Rate { get; set; }
    }
}
