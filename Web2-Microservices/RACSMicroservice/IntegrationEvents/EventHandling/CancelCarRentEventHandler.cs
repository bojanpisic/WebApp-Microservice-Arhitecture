using EventBus.Abstractions;
using RACSMicroservice.IntegrationEvents.Events;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.EventHandling
{
    public class CancelCarRentEventHandler : IIntegrationEventHandler<CancelCarRentEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public CancelCarRentEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(CancelCarRentEvent @event)
        {
            try
            {
                var ress = await unitOfWork.CarRentRepository.Get(crr => crr.TripReservationId == @event.TripReservationId, null, "RentedCar");
                var rent = ress.FirstOrDefault();
                var car = rent.RentedCar;

                if (car == null)
                {
                    return;
                }

                var specialOffers = await unitOfWork.RACSSpecialOfferRepository
                    .Get(s => s.Car == car && s.FromDate == rent.TakeOverDate && s.ToDate == rent.ReturnDate);

                var specOffer = specialOffers.FirstOrDefault();

                if (specOffer != null)
                {
                    specOffer.IsReserved = false;
                }

                if (specOffer != null)
                {
                    unitOfWork.RACSSpecialOfferRepository.Update(specOffer);
                }
                unitOfWork.CarRentRepository.Delete(rent);
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                Console.WriteLine("failed to cancel car rent");
            }
        }
    }
}
