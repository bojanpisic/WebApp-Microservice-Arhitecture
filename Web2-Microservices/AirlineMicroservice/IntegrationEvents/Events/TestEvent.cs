using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class TestEvent : IntegrationEvent
    {
        public string s { get; set; }

        public TestEvent(string s)
        {
            this.s = s;
        }
    }
}
