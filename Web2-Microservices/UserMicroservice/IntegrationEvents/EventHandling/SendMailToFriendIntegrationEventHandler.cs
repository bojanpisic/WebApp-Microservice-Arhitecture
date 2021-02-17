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
    public class SendMailToFriendIntegrationEventHandler : IIntegrationEventHandler<SendMailToFriendIntegrationEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        public SendMailToFriendIntegrationEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(SendMailToFriendIntegrationEvent @event)
        {
            try
            {
                User inviter = (await unitOfWork.UserRepository.GetByID(@event.InviterId)) as User;
                User receiver = (await unitOfWork.UserRepository.GetByID(@event.ReceiverId)) as User;

                await unitOfWork.AuthenticationRepository.SendMailToFriend(inviter, receiver.Email, @event.FlightInfo);
            }
            catch (Exception)
            {
                Console.WriteLine("failed to send mail");
            }

            return;
        }
    }
}
