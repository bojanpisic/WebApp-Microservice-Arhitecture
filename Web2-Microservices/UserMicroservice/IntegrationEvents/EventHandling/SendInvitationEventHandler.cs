using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.IntegrationEvents.Events;
using UserMicroservice.Models;
using UserMicroservice.Repository;

namespace UserMicroservice.IntegrationEvents.EventHandling
{
    public class SendInvitationEventHandler : IIntegrationEventHandler<SendInvitationEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public SendInvitationEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(SendInvitationEvent @event)
        {
            try
            {
                var sender = await unitOfWork.UserRepository.GetByID(@event.InvitationInfo.FirstOrDefault().SenderId) as User;

                var invitationList = new List<Invitation>();

                Dictionary<string, string> receiverInfo = new Dictionary<string, string>();

                foreach (var invitation in @event.InvitationInfo)
                {
                    var receiver = await unitOfWork.UserRepository.GetByID(invitation.ReceiverId) as User;
                    receiverInfo.Add(receiver.Id, receiver.Email);

                    invitationList.Add(new Invitation() 
                    {
                        Sender = sender,
                        Receiver = receiver,
                        Expires = invitation.Expires,
                        Price = invitation.Price,
                        SeatId = invitation.SeatId
                    });


                    sender.TripInvitations.Add(invitationList.Last());
                    receiver.TripRequests.Add(invitationList.Last());

                    unitOfWork.UserRepository.Update(receiver);
                }

                unitOfWork.UserRepository.Update(sender);

                await unitOfWork.Commit();

                //send emails


                foreach (var invitation in @event.InvitationInfo)
                {
                    try
                    {
                        await unitOfWork.AuthenticationRepository.SendMailToFriend(sender, receiverInfo[invitation.ReceiverId], invitation.FlightInfo);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("failed to send email");
                    }
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Failed to process invitation create");
            }
        }
    }
}
