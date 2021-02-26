using EventBus.Abstractions;
using Polly;
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
        private readonly IEventBus eventBus;

        public CreateRACSIntegrationEventHandler(IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.eventBus = eventBus;
        }

        public async Task Handle(CreateRACSIntegrationEvent @event)
        {
            try
            {
               await unitOfWork.RentCarServiceRepository
                    .Insert(
                   new Models.RentACarService() {
                       Name = @event.Name,
                       Address = new Models.Address
                       {
                           State = @event.State,
                           City = @event.City,
                           Lon = @event.Lon,
                           Lat = @event.Lat
                       },
                       AdminId = @event.AdminId,
                   });
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to add racs to database");

                Policy
                    .Handle<Exception>()
                    .Or<ArgumentException>(ex => ex.ParamName == "example")
                    .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(5))
                    .Execute(() => PublishEvent(@event.AdminId));

            }
        }

        private void PublishEvent(string adminId)
        {
            var @event = new RollbackRACSAdmin(adminId);
            eventBus.Publish(@event);
        }
    }
}
