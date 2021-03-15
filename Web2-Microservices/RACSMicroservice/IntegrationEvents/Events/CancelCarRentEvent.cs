using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.Events
{
    public class CancelCarRentEvent: IntegrationEvent
    {
        public CancelCarRentEvent(int tripReservationId)
        {
            TripReservationId = tripReservationId;
        }

        public int TripReservationId { get; set; }
    }
}
