using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.EventHandling
{
    public class RateAirlineIntegrationEventHandler : IIntegrationEventHandler<RateAirlineIntegrationEvent>
    {
        private readonly AirlineRepository airlineRepository;
        private readonly IUnitOfWork unitOfWork;

        public RateAirlineIntegrationEventHandler(AirlineRepository airlineRepository, IUnitOfWork unitOfWork)
        {
            this.airlineRepository = airlineRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(RateAirlineIntegrationEvent @event) 
        {
            var airlines = await airlineRepository.Get(a => a.AirlineId == @event.AirlineId, null, "Rates");

            var airline = airlines.FirstOrDefault();

            if (airline == null)
            {
                return;
            }

            if (airline.Rates.FirstOrDefault(r => r.UserId == @event.Id.ToString()) != null)
            {
                return;
            }
            //PROVERA DA LI JE LETIO OVOM KOMPANIJOM
            //var flightReservations = await unitOfWork.FlightReservationRepository
            //                                .Get(f => f.Tickets.FirstOrDefault(t =>
            //                                                                    t.Seat.Flight.Airline == airline
            //                                                                    && t.Seat.Flight.LandingDateTime >= DateTime.Now) != null
            //                                                                    && f.User == user,
            //                                null,
            //                                "Tickets");

            airline.Rates.Add(new AirlineRate()
            {
                Rate = @event.Rate,
                UserId = @event.Id.ToString(),
                Airline = airline
            });

            try
            {
                airlineRepository.Update(airline);

                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to rate airline");
            }
        }
    }
}
