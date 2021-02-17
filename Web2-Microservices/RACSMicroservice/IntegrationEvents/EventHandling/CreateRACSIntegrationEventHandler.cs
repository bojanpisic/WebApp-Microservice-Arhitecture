using EventBus.Abstractions;
using RACSMicroservice.IntegrationEvents.Events;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.EventHandling
{
    public class CreateRACSIntegrationEventHandler : IIntegrationEventHandler<CreateRACSIntegrationEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public CreateRACSIntegrationEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task Handle(CreateRACSIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
