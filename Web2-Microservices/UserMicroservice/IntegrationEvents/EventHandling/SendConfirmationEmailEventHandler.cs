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
    public class SendConfirmationEmailEventHandler : IIntegrationEventHandler<SendConfirmationEmailEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public SendConfirmationEmailEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(SendConfirmationEmailEvent @event)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(@event.ReceiverEmail) as User;
            await unitOfWork.AuthenticationRepository
                .SendRentConfirmationMail(user.Email,
                new DTOs.RentInfo(@event.TakeOverCity, @event.ReturnCity, @event.TotalPrice, @event.RentDate, @event.TakeOverDate, @event.ReturnDate,
                @event.Brand, @event.Model, @event.Type, @event.PricePerDay));
        }
    }                          
}
