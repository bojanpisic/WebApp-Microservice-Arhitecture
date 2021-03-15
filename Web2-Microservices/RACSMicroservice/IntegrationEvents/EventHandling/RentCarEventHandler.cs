using EventBus.Abstractions;
using Microsoft.EntityFrameworkCore;
using RACSMicroservice.IntegrationEvents.Events;
using RACSMicroservice.Models;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.EventHandling
{
    public class RentCarEventHandler : IIntegrationEventHandler<RentCarEvent>
    {
        private readonly IUnitOfWork unitOfWork;

        public RentCarEventHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(RentCarEvent @event)
        {
            try
            {


                Car car = (await unitOfWork.CarRepository.AllCars(c => c.CarId == @event.CarId)).FirstOrDefault();

                if (car == null)
                {
                    //return NotFound("Car not found");
                }

                //provera da li ima special offer za taj period
                var specialOffer = car.SpecialOffers.FirstOrDefault(so =>
                                    @event.ReturnDate >= so.FromDate && so.FromDate >= @event.TakeOverDate ||
                                    @event.ReturnDate >= so.ToDate && so.FromDate >= @event.TakeOverDate ||
                                    so.FromDate < @event.TakeOverDate && so.ToDate > @event.ReturnDate);

                if (specialOffer != null)
                {

                    return;
                    //return BadRequest("This car has special offer for selected period. Cant rent this car");
                }

                //provera da li je vec rezervisan u datom periodu
                foreach (var rent in car.Rents)
                {
                    if (!(rent.TakeOverDate < @event.TakeOverDate && rent.ReturnDate < @event.TakeOverDate ||
                        rent.TakeOverDate > @event.ReturnDate && rent.ReturnDate > @event.ReturnDate))
                    {
                        return;
                        //return BadRequest("The selected car is reserved for selected period");
                    }
                }

                //var racsId = car.BranchId == null ? car.RentACarServiceId : car.Branch.RentACarServiceId;

                //var result = await unitOfWork.RentACarRepository.Get(r => r.RentACarServiceId == racsId, null, "Address,Branches");
                //var racs = result.FirstOrDefault();

                //if (racs == null)
                //{
                //    return NotFound("RACS not found");
                //}
                if (car.Branch != null)
                {
                    if (!car.Branch.City.Equals(@event.TakeOverCity))
                    {
                        //return BadRequest("Takeover city and rent service/branch city dont match");
                        return;

                    }
                }
                else
                {
                    if (!car.RentACarService.Address.City.Equals(@event.TakeOverCity))
                    {
                        //return BadRequest("Takeover city and rent service/branch city dont match");
                        return;
                    }
                }

                //provera da li postoje branch gde moze da se vrati auto

                var citiesToReturn = new List<string>();

                foreach (var item in car.RentACarService == null ? car.Branch.RentACarService.Branches : car.RentACarService.Branches)
                {
                    citiesToReturn.Add(item.City);
                }

                citiesToReturn.Add(car.RentACarService == null ? car.Branch.RentACarService.Address.City : car.RentACarService.Address.City);

                if (!citiesToReturn.Contains(@event.ReturnCity))
                {
                    return;
                }

                var carRent = new CarRent()
                {
                    TakeOverCity = @event.TakeOverCity,
                    ReturnCity = @event.ReturnCity,
                    TakeOverDate = @event.TakeOverDate,
                    ReturnDate = @event.ReturnDate,
                    RentedCar = car,
                    UserId = @event.UserId,
                    TotalPrice = (await CalculateTotalPrice(@event.TakeOverDate, @event.ReturnDate, car.PricePerDay)),
                    RentDate = DateTime.Now,
                    TripReservationId = @event.TripReservationId
                };

                car.Rents.Add(carRent);
                unitOfWork.CarRepository.Update(car);
                await unitOfWork.Commit();
            }
            catch (DbUpdateConcurrencyException) 
            {

            }
            catch (DbUpdateException)
            {
            }
            catch (Exception)
            {

            }
        }

        private async Task<float> CalculateTotalPrice(DateTime startDate, DateTime endDate, float pricePerDay)
        {
            await Task.Yield();

            return (float)((endDate - startDate).TotalDays == 0 ? pricePerDay : pricePerDay * (endDate - startDate).TotalDays);
        }
    }
}
