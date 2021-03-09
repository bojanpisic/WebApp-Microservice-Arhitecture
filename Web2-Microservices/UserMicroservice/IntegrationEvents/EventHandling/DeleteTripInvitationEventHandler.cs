using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.IntegrationEvents.Events;
using UserMicroservice.Repository;

namespace UserMicroservice.IntegrationEvents.EventHandling
{
    public class DeleteTripInvitationEventHandler : IIntegrationEventHandler<DeleteTripInvitation>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteTripInvitationEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(DeleteTripInvitation @event)
        {
            try
            {
                var invitation = await unitOfWork.TripInvitationRepository.GetTripInvitationById(@event.InvitationId);
                
                invitation.Receiver.TripRequests.Remove(invitation);
                invitation.Sender.TripInvitations.Remove(invitation);

                unitOfWork.TripInvitationRepository.Delete(invitation);

                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                Console.WriteLine("failed to process delete invitation event");
            }
        }
    }
}
