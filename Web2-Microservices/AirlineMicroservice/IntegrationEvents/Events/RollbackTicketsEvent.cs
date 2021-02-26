using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class RollbackTicketsEvent:IntegrationEvent
    {
        public List<int> TicketIds { get; set; }

        public RollbackTicketsEvent(List<int> ticketIds)
        {
            TicketIds = ticketIds;
        }
    }
}
