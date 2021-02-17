using AirlineMicroservice.IntegrationEvents.Events;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.EventHandling
{
    public class TestEventHandler : IIntegrationEventHandler<TestEvent>
    {
        public Task Handle(TestEvent @event)
        {
            Console.WriteLine("\n\n\nUSEPSNO USAO U HANDLE\n\n\n");

            return Task.CompletedTask;
        }
    }
}
