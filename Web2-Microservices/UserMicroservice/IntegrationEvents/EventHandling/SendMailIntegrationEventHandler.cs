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
    public class SendMailIntegrationEventHandler : IIntegrationEventHandler<SendMailIntegrationEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        public SendMailIntegrationEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(SendMailIntegrationEvent @event)
        {
            try
            {
                User user = (await unitOfWork.UserRepository.GetByID(@event.UserId)) as User;
                await unitOfWork.AuthenticationRepository.SendTicketConfirmationMail(user.Email, @event.TripInfo);
            }
            catch (Exception)
            {
                Console.WriteLine("failed to send mail");
            }

            return;
        }
    }
}
