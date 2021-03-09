using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.EventHandling
{
    public class RejectTripInvitationHandler : IIntegrationEventHandler<RejectTripInvitationEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventBus eventBus;

        public RejectTripInvitationHandler(IUnitOfWork unitOfWork,IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.eventBus = eventBus;
        }
        public async Task Handle(RejectTripInvitationEvent @event)
        {
            try
            {
                var seat = await unitOfWork.SeatRepository.GetSeatWithTicket(@event.SeatId);

                seat.Available = true;
                seat.Reserved = false;

                unitOfWork.SeatRepository.Update(seat);
                unitOfWork.TicketRepository.Delete(seat.Ticket);

                await unitOfWork.Commit();

                var @event1 = new DeleteTripInvitation(@event.InvitationId);
                eventBus.Publish(@event1);
            }
            catch (Exception)
            {
                Console.WriteLine("FAILED TO PROCESS REJECT TRIP INVITATION EVENT");
            }
        }
    }
}
