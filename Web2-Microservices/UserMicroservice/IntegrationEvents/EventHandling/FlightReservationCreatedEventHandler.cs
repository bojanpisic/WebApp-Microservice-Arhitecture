using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.IntegrationEvents.Events;
using UserMicroservice.Repository;

namespace UserMicroservice.IntegrationEvents.EventHandling
{
    public class FlightReservationCreatedEventHandler : IIntegrationEventHandler<FlightReservationCreatedEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public FlightReservationCreatedEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(FlightReservationCreatedEvent @event)
        {
            try
            {
                var systemB = await unitOfWork.BonusRepository.GetByID(1);

                int systemBonus = 0;

                if (systemB == null)
                {
                    systemBonus = 0;
                }
                else
                {
                    systemBonus = systemB.BonusPerKilometer;
                }


                var invitation = await unitOfWork.TripInvitationRepository.GetTripInvitationById(@event.InvitationForDeleteId);

                invitation.Receiver.BonusPoints += (int)@event.TripLength * systemBonus;

                invitation.Receiver.TripRequests.Remove(invitation);
                invitation.Sender.TripInvitations.Remove(invitation);

                unitOfWork.UserRepository.Update(invitation.Receiver);
                unitOfWork.UserRepository.Update(invitation.Sender);

                unitOfWork.TripInvitationRepository.Delete(invitation);

                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                Console.WriteLine("failed to handle flight reservation created event");
            }
           
        }
    }
}
