using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.EventHandling
{
    public class RollbackTicketsEventHandler : IIntegrationEventHandler<RollbackTicketsEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public RollbackTicketsEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(RollbackTicketsEvent @event)
        {
            int counter = 0;
            while (counter != 10) 
            {
                try
                {
                    foreach (var ticketId in @event.TicketIds)
                    {
                        var ticket = await unitOfWork.TicketRepository.GetByID(ticketId);
                        var seat = ticket.Seat;
                        seat.Available = true;
                        seat.Reserved = false;

                        unitOfWork.SeatRepository.Update(seat);
                        unitOfWork.TicketRepository.Delete(ticketId);
                    }

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    counter++;
                    Thread.Sleep(5000);
                }
            }
           
        }
    }
}
