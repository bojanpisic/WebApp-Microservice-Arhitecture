using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly;
using RACSMicroservice.DTOs;
using RACSMicroservice.IntegrationEvents.Events;
using RACSMicroservice.Models;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RACSMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;
        private readonly IEventBus eventBus;

        public ReservationController(IUnitOfWork unitOfWork, HttpClient httpClient,IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.httpClient = httpClient;
            this.eventBus = eventBus;
        }
        #region Rent methods
        [HttpPost]
        [Route("rent-car")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RentCar(CarRentDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }


                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                if (dto.TakeOverDate < DateTime.Now.Date)
                {
                    return BadRequest("Date is in past");
                }

                if (dto.TakeOverDate > dto.ReturnDate)
                {
                    return BadRequest("Takeover date shoud be lower then return date.");
                }

                var car = (await unitOfWork.CarRepository.AllCars(c => c.CarId == dto.CarRentId)).FirstOrDefault();

                if (car == null)
                {
                    return NotFound("Car not found");
                }

                //provera da li ima special offer za taj period
                var specialOffer = car.SpecialOffers.FirstOrDefault(so =>
                                    dto.ReturnDate >= so.FromDate && so.FromDate >= dto.TakeOverDate ||
                                    dto.ReturnDate >= so.ToDate && so.FromDate >= dto.TakeOverDate ||
                                    so.FromDate < dto.TakeOverDate && so.ToDate > dto.ReturnDate);

                if (specialOffer != null)
                {
                    return BadRequest("This car has special offer for selected period. Cant rent this car");
                }

                //provera da li je vec rezervisan u datom periodu
                foreach (var rent in car.Rents)
                {
                    if (!(rent.TakeOverDate < dto.TakeOverDate && rent.ReturnDate < dto.TakeOverDate ||
                        rent.TakeOverDate > dto.ReturnDate && rent.ReturnDate > dto.ReturnDate))
                    {
                        return BadRequest("The selected car is reserved for selected period");
                    }
                }

                //var racsId = car.BranchId == null ? car.RentACarServiceId : car.Branch.RentACarServiceId;

                //var result = await unitOfWork.RentACarRepository.Get(r => r.RentACarServiceId ==racsId, null, "Address,Branches");
                //var racs = result.FirstOrDefault();

                //if (racs == null)
                //{
                //    return NotFound("RACS not found");
                //}
                if (car.Branch != null)
                {
                    if (!car.Branch.City.Equals(dto.TakeOverCity))
                    {
                        return BadRequest("Takeover city and rent service/branch city dont match");

                    }
                }
                else
                {
                    if (!car.RentACarService.Address.City.Equals(dto.TakeOverCity))
                    {
                        return BadRequest("Takeover city and rent service/branch city dont match");
                    }
                }

                //provera da li postoje branch gde moze da se vrati auto

                var citiesToReturn = new List<string>();

                foreach (var item in car.RentACarService == null ? car.Branch.RentACarService.Branches : car.RentACarService.Branches)
                {
                    citiesToReturn.Add(item.City);
                }

                citiesToReturn.Add(car.RentACarService == null ? car.Branch.RentACarService.Address.City : car.RentACarService.Address.City);

                if (!citiesToReturn.Contains(dto.ReturnCity))
                {
                    return BadRequest("Cant return to selected city");
                }

                var carRent = new CarRent()
                {
                    TakeOverCity = dto.TakeOverCity,
                    ReturnCity = dto.ReturnCity,
                    TakeOverDate = dto.TakeOverDate,
                    ReturnDate = dto.ReturnDate,
                    RentedCar = car,
                    UserId = userId,
                    TotalPrice = await CalculateTotalPrice(dto.TakeOverDate, dto.ReturnDate, car.PricePerDay),
                    RentDate = DateTime.Now
                };

                car.Rents.Add(carRent);
                
                unitOfWork.CarRepository.Update(car);
                await unitOfWork.Commit();

                //slanje email-a
                await Policy
                  .Handle<Exception>()
                  .Or<ArgumentException>(ex => ex.ParamName == "example")
                  .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(5))
                  .ExecuteAsync(async () => await SendConfirmationEmail(userId,carRent));

                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest("Car is modified in the meantime, or reserved by another user");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rent car");
            }
        }

        private  Task SendConfirmationEmail(string userId, CarRent info) 
        {
            var @event = new SendConfirmationEmailEvent(userId, info.TakeOverCity, info.ReturnCity, info.TotalPrice, info.RentDate, info.TakeOverDate,
                info.ReturnDate, info.RentedCar.Brand, info.RentedCar.Model, info.RentedCar.Type, info.RentedCar.PricePerDay);

            eventBus.Publish(@event);
            return Task.CompletedTask;
        }

        [HttpGet]
        [Route("rent-total-price")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetTotalPrice()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }
                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }
                var queryString = Request.Query;
                var carId = 0;
                DateTime retDate;
                DateTime takeDate;

                if (!Int32.TryParse(queryString["carId"].ToString(), out carId))
                {
                    return BadRequest();
                }
                if (!DateTime.TryParse(queryString["ret"].ToString(), out retDate))
                {
                    return BadRequest();
                }
                if (!DateTime.TryParse(queryString["dep"].ToString(), out takeDate))
                {
                    return BadRequest();
                }
                if (retDate < takeDate)
                {
                    return BadRequest();
                }

                var car = await unitOfWork.CarRepository.GetByID(carId);

                if (car == null)
                {
                    return NotFound("Car not found");
                }
                //ovde bi se trebali uracunati i bodovi korisnika, kako bi se uracunala snizenja
                var totalPrice = await CalculateTotalPrice(takeDate, retDate, car.PricePerDay);

                //var returnData = new {
                //    from = queryString["from"].ToString(),
                //    to = queryString["to"].ToString(),
                //    dep = takeDate,
                //    ret = retDate,
                //    brand = car.Brand,
                //    carId = car.CarId,
                //    model = car.Model, =
                //    name = car.
                //};

                return Ok(totalPrice);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return total price");
            }
        }

        [HttpDelete]
        [Route("cancel-rent/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CancelRent(int id)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var ress = await unitOfWork.CarRentRepository.Get(crr => crr.CarRentId == id, null, "RentedCar");
                var rent = ress.FirstOrDefault();
                var car = rent.RentedCar;

                if (car == null)
                {
                    return NotFound("Car not found");
                }

                if (rent.UserId != userId)
                {
                    return BadRequest();
                }

                if (rent.TakeOverDate.AddDays(-2) < DateTime.Now.Date)
                {
                    return BadRequest("Cant cancel reservation");
                }

                var specialOffers = await unitOfWork.RACSSpecialOfferRepository.Get(s => s.Car == car && s.FromDate == rent.TakeOverDate && s.ToDate == rent.ReturnDate);
                var specOffer = specialOffers.FirstOrDefault();

                if (specOffer != null)
                {
                    specOffer.IsReserved = false;
                }

                //car.Rents.Remove(rent);
                
                if (specOffer != null)
                {
                    unitOfWork.RACSSpecialOfferRepository.Update(specOffer);
                }
                unitOfWork.CarRentRepository.Delete(rent);
                //unitOfWork.CarRepository.Update(car);

                await unitOfWork.Commit();

                return Ok();
            }
            catch(DbUpdateException)
            {
                Console.WriteLine("Remove rent Transaction failed");
                return StatusCode(500, "Failed to cancel rent car");

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to cancel rent car");
            }
        }

        [HttpGet]
        [Route("get-car-reservations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRents()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var rents = await unitOfWork.CarRentRepository.GetRents(userId);
                var retVal = new List<object>();
                Console.WriteLine(rents.Count());
                foreach (var rent in rents)
                {
                    var sum = 0.0;
                    foreach (var item in rent.RentedCar.Rates)
                    {
                        sum += item.Rate;
                    }

                    var rate = sum == 0.0 ? 0.0 : sum / rent.RentedCar.Rates.Count;

                    retVal.Add(new
                    {
                        brand = rent.RentedCar.Brand,
                        carId = rent.RentedCar.CarId,
                        model = rent.RentedCar.Model,
                        seatsNumber = rent.RentedCar.SeatsNumber,
                        pricePerDay = rent.RentedCar.PricePerDay,
                        type = rent.RentedCar.Type,
                        year = rent.RentedCar.Year,
                        totalPrice = rent.TotalPrice,
                        from = rent.TakeOverCity,
                        to = rent.ReturnCity,
                        dep = rent.TakeOverDate,
                        ret = rent.ReturnDate,
                        carServiceId = rent.RentedCar.RentACarService == null ?
                                       rent.RentedCar.Branch.RentACarService.RentACarServiceId : rent.RentedCar.RentACarService.RentACarServiceId,
                        name = rent.RentedCar.RentACarService == null ?
                                       rent.RentedCar.Branch.RentACarService.Name : rent.RentedCar.RentACarService.Name,
                        city = rent.RentedCar.RentACarService == null ?
                                       rent.RentedCar.Branch.RentACarService.Address.City : rent.RentedCar.RentACarService.Address.City,
                        state = rent.RentedCar.RentACarService == null ?
                                       rent.RentedCar.Branch.RentACarService.Address.State : rent.RentedCar.RentACarService.Address.State,

                        isCarRated = rent.RentedCar.Rates.FirstOrDefault(r => r.UserId == userId) != null ? true : false,
                        isRACSRated = (await unitOfWork.RentCarServiceRepository.Get(r => r.RentACarServiceId == (rent.RentedCar.RentACarService == null ?
                                       rent.RentedCar.Branch.RentACarService.RentACarServiceId : rent.RentedCar.RentACarService.RentACarServiceId), null, "Rates"))
                                       .FirstOrDefault().Rates.FirstOrDefault(r => r.UserId == userId) == null ? false : true,
                        canCancel = rent.TakeOverDate.AddDays(-2) >= DateTime.Now.Date,
                        canRate = rent.ReturnDate < DateTime.Now.Date,
                        isUpcoming = rent.TakeOverDate >= DateTime.Now.Date,
                        reservationId = rent.CarRentId,
                        rate = rate
                    });
                }

                return Ok(retVal);
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to return car reservations");

                Console.WriteLine(ex.Message);
                return StatusCode(500, "Something went wrong");
            }
        }

        private async Task<float> CalculateTotalPrice(DateTime startDate, DateTime endDate, float pricePerDay)
        {
            await Task.Yield();

            return (float)((endDate - startDate).TotalDays == 0 ? pricePerDay : pricePerDay * (endDate - startDate).TotalDays);
        }
        #endregion

        [HttpPost]
        [Route("reserve-special-offer-car")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ReserveSpecialOfferCar([FromBody] ReserveDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var specialOffer = await unitOfWork.RACSSpecialOfferRepository.GetSpecialOfferById(dto.Id);

                if (specialOffer == null)
                {
                    return NotFound("Selected special offer is not found");
                }

                if (specialOffer.IsReserved)
                {
                    return BadRequest("Already reserved");
                }

                foreach (var rent in specialOffer.Car.Rents)
                {
                    if (!(rent.TakeOverDate < specialOffer.FromDate && rent.ReturnDate < specialOffer.FromDate ||
                        rent.TakeOverDate > specialOffer.ToDate && rent.ReturnDate > specialOffer.ToDate))
                    {
                        return BadRequest("The selected car is reserved for selected period");
                    }
                }

                var carRent = new CarRent()
                {
                    TakeOverDate = specialOffer.FromDate,
                    ReturnDate = specialOffer.ToDate,
                    TakeOverCity = specialOffer.Car.RentACarService == null ?
                                    specialOffer.Car.Branch.City : specialOffer.Car.RentACarService.Address.City,
                    ReturnCity = specialOffer.Car.RentACarService == null ?
                                    specialOffer.Car.Branch.City : specialOffer.Car.RentACarService.Address.City,
                    RentedCar = specialOffer.Car,
                    TotalPrice = specialOffer.NewPrice,
                    RentDate = DateTime.Now
                };

                specialOffer.Car.Rents.Add(carRent);
                specialOffer.IsReserved = true;

                unitOfWork.CarRepository.Update(specialOffer.Car);
                unitOfWork.RACSSpecialOfferRepository.Update(specialOffer);

                await unitOfWork.Commit();

                //slanje email-a
                //slanje email-a
                await Policy
                  .Handle<Exception>()
                  .Or<ArgumentException>(ex => ex.ParamName == "example")
                  .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(5))
                  .ExecuteAsync(async () => await SendConfirmationEmail(userId, carRent));

                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest("Car is modified in the meantime, or reserved by another user");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to reserve special offer");
            }
        }
    }
}
