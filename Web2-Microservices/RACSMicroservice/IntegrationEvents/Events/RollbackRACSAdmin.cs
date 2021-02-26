using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.Events
{
    public class RollbackRACSAdmin: IntegrationEvent
    {
        public string AdminId { get; set; }

        public RollbackRACSAdmin(string adminId)
        {
            AdminId = adminId;
        }
    }
}
