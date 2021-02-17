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
    public class RateFlightIntegrationEventHandler : IIntegrationEventHandler<RateFlightIntegrationEvent>
    {
        private readonly IFlightRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public RateFlightIntegrationEventHandler(IFlightRepository repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(RateFlightIntegrationEvent @event)
        {
            var flights = await unitOfWork.FlightRepository.Get(f => f.FlightId == @event.FlightId, null, "Rates");

            var flight = flights.FirstOrDefault();

            if (flight == null)
            {
                return;
            }

            if (flight.Rates.FirstOrDefault(r => r.UserId == @event.UserId) != null)
            {
                return;
            }

            //PROVERA DA LI JE LETIO NA OVOM LETU
            //var flightReservations = await unitOfWork.FlightReservationRepository
            //                                .Get(f => f.Tickets.FirstOrDefault(t => t.Seat.Flight == flight) != null && f.User == user,
            //                                null,
            //                                "Tickets");

            //var flightReservation = flightReservations.FirstOrDefault();

            //if (flightReservation == null)
            //{
            //    return BadRequest("You didnt reserve seat on this flight. Cant rate.");
            //}

            //var ticketOfReservation = flightReservation.Tickets.FirstOrDefault();
            //if (ticketOfReservation == null)
            //{
            //    return BadRequest();
            //}

            //var ticket = await unitOfWork.TicketRepository.GetTicket(ticketOfReservation.TicketId);

            //if (ticket == null)
            //{
            //    return BadRequest();
            //}

            //if (ticket.Seat.Flight.LandingDateTime >= DateTime.Now)
            //{
            //    return BadRequest("You can rate flight after landing");
            //}

            flight.Rates.Add(new FlightRate()
            {
                Rate = @event.Rate,
                UserId = @event.UserId,
                Flight = flight
            });

            try
            {
                unitOfWork.FlightRepository.Update(flight);

                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to rate flight");
            }
        }
    }
}
