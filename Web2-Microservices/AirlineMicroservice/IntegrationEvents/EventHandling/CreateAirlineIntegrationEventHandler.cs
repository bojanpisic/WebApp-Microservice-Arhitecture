using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using System;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.EventHandling
{
    public class CreateAirlineIntegrationEventHandler : IIntegrationEventHandler<CreateAirlineIntegrationEvent>
    {
        private readonly AirlineRepository airlineRepository;

        public CreateAirlineIntegrationEventHandler(AirlineRepository airlineRepository)
        {
            this.airlineRepository = airlineRepository;
        }
        public async Task Handle(CreateAirlineIntegrationEvent @event)
        {
            try
            {
                await airlineRepository.Insert
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
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to add airline to database");
            }

        }

    }
}
