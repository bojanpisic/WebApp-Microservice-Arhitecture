using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using Polly;
using System;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.EventHandling
{
    public class CreateAirlineIntegrationEventHandler : IIntegrationEventHandler<CreateAirlineIntegrationEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventBus eventBus;

        public CreateAirlineIntegrationEventHandler(IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.eventBus = eventBus;
        }
        public async Task Handle(CreateAirlineIntegrationEvent @event)
        {
            try
            {
                await unitOfWork.AirlineRepository.Insert
                        (new Models.Airline
                        {
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

                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                Policy
                    .Handle<Exception>()
                    .Or<ArgumentException>(ex => ex.ParamName == "example")
                    .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(5))
                    .Execute(() => PublishEvent(@event.AdminId));

                Console.WriteLine("Failed to add airline to database");
            }

        }

        private void PublishEvent(string adminId) 
        {
            var @event = new RollbackAirlineAdmin(adminId);
            eventBus.Publish(@event);
        }

    }
}
