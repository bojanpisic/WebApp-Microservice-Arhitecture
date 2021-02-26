using EventBus.Abstractions;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.IntegrationEvents.Events;
using UserMicroservice.Repository;

namespace UserMicroservice.IntegrationEvents.EventHandling
{
    public class RollbackAirlineAdminHandler : IIntegrationEventHandler<RollbackAirlineAdmin>
    {
        private readonly IUnitOfWork unitOfWork;

        public RollbackAirlineAdminHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public Task Handle(RollbackAirlineAdmin @event)
        {

            Policy
                .Handle<Exception>()
                .Or<ArgumentException>(ex => ex.ParamName == "example")
                .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(5))
                .Execute(() => RemoveAdmin(@event.AdminId));

            return Task.CompletedTask;
        }

        private async void RemoveAdmin(string adminId)
        {
           await unitOfWork.AuthenticationRepository.DeleteUserById(adminId);
        }
    }
}
