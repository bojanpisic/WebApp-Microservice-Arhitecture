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
    public class CreateFlightReservationEventHandler : IIntegrationEventHandler<CreateFlightReservationEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventBus eventBus;

        public CreateFlightReservationEventHandler(IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.eventBus = eventBus;
        }
        public async Task Handle(CreateFlightReservationEvent @event)
        {
            try
            {
                var flightReservation = new FlightReservation()
                {
                    UserId = @event.UserId,
                };

                var tickets = new List<Ticket>();

                var seat = await unitOfWork.SeatRepository.GetSeatWithTicket(@event.SeatId);

                seat.Ticket.Passport = @event.Passport;
                seat.Ticket.Reservation = flightReservation;

                tickets.Add(seat.Ticket);

                flightReservation.Tickets = tickets;

                await unitOfWork.FlightReservationRepository.Insert(flightReservation);
                await unitOfWork.Commit();

                var @event1 = new FlightReservationCreatedEvent(@event.InvitationId, seat.Flight.tripLength);

                eventBus.Publish(@event1);

            }
            catch (Exception)
            {
                Console.WriteLine("Failed to create flight reservation");
            }
        }
    }
}
