using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.IntegrationEvents.Events
{
    public class RollbackAirlineAdmin : IntegrationEvent
    {
        public string AdminId { get; set; }
        public RollbackAirlineAdmin(string adminId)
        {
            AdminId = adminId;
        }

    }
}
