using AirlineMicroservice.DTOs;
using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirlineMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlineController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;
        private readonly IEventBus eventBus;

        public AirlineController(IUnitOfWork _unitOfWork, HttpClient httpClient, IEventBus eventBus)
        {
            this.unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
            this.eventBus = eventBus;
        }

        

        //[HttpPost]
        //[Route("create-airline")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> CreateAirline([FromBody] AirlineInfoDto registerDto) 
        //{
        //    try
        //    {

        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("SystemAdmin"))
        //        {
        //            return Unauthorized();
        //        }

        //        HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

        //        if (result.Equals(HttpStatusCode.NotFound))
        //        {
        //            return NotFound();
        //        }

        //        var airline = new Airline()
        //        {
        //            Name = registerDto.Name,
        //            Address = new Address()
        //            {
        //                City = registerDto.City,
        //                State = registerDto.State,
        //                Lat = registerDto.Lat,
        //                Lon = registerDto.Lon
        //            },
        //            AdminId = registerDto.AdminId
        //        };

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        var @event = new RollbackAirlineAdmin(registerDto.AdminId);
        //        eventBus.Publish(@event);

        //        return StatusCode(500, "Failed to create airline");
        //    }
            
        //}

        [HttpGet]
        [Route("get-airline")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAdminsAirline()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }
                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId, null, "Flights,Address");

                var airline = res.FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                object obj = new
                {
                    airline.Name,
                    airline.PromoDescription,
                    airline.Address,
                    airline.LogoUrl
                };

                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return airline.");
            }
        }


        #region Change airline info methods

        [HttpPut]
        [Route("change-airline-info")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeAirlineInfo([FromBody] ChangeAirlineInfoDto airlineInfoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId)))
                                            .StatusCode;


                if (result.Equals(HttpStatusCode.NotFound))
                {
                    Console.WriteLine("USER NOT FOUND WITH ID " + userId);
                    return NotFound("Something went wrong");
                }

                var airline = (await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId, null, "Address")).FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found.");
                }

                airline.Name = airlineInfoDto.Name;
                airline.PromoDescription = airlineInfoDto.PromoDescription;

                bool addressChanged = false;

                if (!airline.Address.City.Equals(airlineInfoDto.Address.City) ||
                    !airline.Address.State.Equals(airlineInfoDto.Address.State)
                    || !airline.Address.Lat.Equals(airlineInfoDto.Address.Lat) ||
                    !airline.Address.Lon.Equals(airlineInfoDto.Address.Lon))
                {
                    airline.Address.City = airlineInfoDto.Address.City;
                    airline.Address.State = airlineInfoDto.Address.State;
                    airline.Address.Lon = airlineInfoDto.Address.Lon;
                    airline.Address.Lat = airlineInfoDto.Address.Lat;
                    addressChanged = true;
                }

                var r = IdentityResult.Success;
                if (addressChanged)
                {

                    unitOfWork.AirlineRepository.Update(airline);
                    unitOfWork.AirlineRepository.UpdateAddress(airline.Address);

                    await unitOfWork.Commit();
                }
                else
                {
                    unitOfWork.AirlineRepository.Update(airline);
                    await unitOfWork.Commit();
                }

                if (r.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(r.Errors);
            }

            catch (DbUpdateException)
            {
                Console.WriteLine("Transaction failed");
                return StatusCode(500, "Failed to change airline info");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Failed to change airline info");
            }
        }

        [HttpPut]
        [Route("change-airline-logo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeAirlineLogo(IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    Console.WriteLine("USER NOT FOUND WITH ID " + userId);
                    return NotFound("Something went wrong");
                }

                var findResult = await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId);
                var airline = findResult.FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline doesnt exist");
                }

                using (var stream = new MemoryStream())
                {
                    await img.CopyToAsync(stream);
                    airline.LogoUrl = stream.ToArray();
                }

                unitOfWork.AirlineRepository.Update(airline);
                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Transaction failed");
                return StatusCode(500, "Failed to change.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to change.");
            }
        }

        #endregion

        #region Special offer methods

        [HttpGet]
        [Route("get-specialoffer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetSpecialOffers()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }
                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var airline = (await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId)).FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found.");
                }

                var specOffers = await unitOfWork.SpecialOfferRepository.GetSpecialOffersOfAirline(airline);

                List<object> objs = new List<object>();
                List<object> flights = new List<object>();
                List<object> fstops = new List<object>();


                foreach (var item in specOffers)
                {
                    flights = new List<object>();

                    foreach (var seat in item.Seats)
                    {
                        fstops = new List<object>();

                        foreach (var stop in seat.Flight.Stops)
                        {
                            fstops.Add(new
                            {
                                city = stop.Destination.City
                            });
                        }

                        flights.Add(new
                        {
                            flightId = seat.Flight.FlightId,
                            flightNumber = seat.Flight.FlightNumber,
                            to = seat.Flight.To.City,
                            from = seat.Flight.From.City,
                            tripLength = seat.Flight.tripLength,
                            tripTime = seat.Flight.TripTime,
                            stops = fstops,
                            landingDate = seat.Flight.LandingDateTime.Date,
                            landingTime = seat.Flight.LandingDateTime.TimeOfDay,
                            takeOffDate = seat.Flight.TakeOffDateTime.Date,
                            takeOffTime = seat.Flight.TakeOffDateTime.TimeOfDay,
                            seat.Class,
                            seat.Column,
                            seat.Row,
                            seat.Price,
                            seat.Reserved,
                            seat.SeatId
                        }
                        );
                    }

                    objs.Add(new
                    {
                        airline.LogoUrl,
                        airline.Name,
                        item.NewPrice,
                        item.OldPrice,
                        item.SpecialOfferId,
                        flights
                    });
                }

                return Ok(objs);

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return special offers.");
            }

        }

        [HttpPost]
        [Route("add-specialoffer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddSpecialOffer([FromBody] SpecialOfferDto specialOfferDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (specialOfferDto.SeatsIds.Count == 0)
            {
                return BadRequest();
            }
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                if (specialOfferDto.NewPrice < 0)
                {
                    return BadRequest("Price should be greater then 0");
                }

                var airline = (await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId)).FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                List<Seat> seats = new List<Seat>();
                float oldPrice = 0;

                foreach (var seatId in specialOfferDto.SeatsIds)
                {
                    var seatt = (await unitOfWork.SeatRepository.Get(s => s.SeatId == seatId, null, "SpecialOffer")).FirstOrDefault();

                    if (seatt == null)
                    {
                        return BadRequest("Something went wrong");
                    }

                    if (seatt.SpecialOffer != null)
                    {
                        return BadRequest("Seat have special offer already");
                    }

                    oldPrice += seatt.Price;
                    seats.Add(seatt);
                }
                if (seats.Count == 0 || seats.Count != specialOfferDto.SeatsIds.Count)
                {
                    return BadRequest("Something went wrong");
                }

                var specialOffer = new SpecialOffer()
                {
                    Airline = airline,
                    OldPrice = oldPrice,
                    NewPrice = specialOfferDto.NewPrice,
                    Seats = seats
                };

                airline.SpecialOffers.Add(specialOffer);


                await unitOfWork.SpecialOfferRepository.Insert(specialOffer);
                unitOfWork.AirlineRepository.Update(airline);

                foreach (var seat in seats)
                {
                    seat.Available = false;
                    unitOfWork.SeatRepository.Update(seat);
                }

                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Concurrency exception thrown");
                return StatusCode(500, "Failed to add special offer");
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Transaction failed");
                return StatusCode(500, "Failed to add special offer");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add special offer");
            }
        }

        [HttpDelete]
        [Route("delete-specialoffer/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> DeleteSpecialOffer(int id)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var airline = (await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId)).FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                var specOffer = await unitOfWork.SpecialOfferRepository.GetByID(id);

                if (specOffer == null)
                {
                    return NotFound("Special offer not found");
                }

                unitOfWork.SpecialOfferRepository.Delete(specOffer);
                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Concurrency exception thrown");
                return StatusCode(500, "Failed to delete special offer");

            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Transaction failed");
                return StatusCode(500, "Failed to delete special offer");

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete special offer");
            }

        }

        #endregion


        [HttpPost]
        [Route("rate-airline")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RateAirline(RateDto dto)
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
                if (dto.Rate > 5 || dto.Rate < 1)
                {
                    return BadRequest("Invalid rate. Rate from 1 to 5");
                }

                var @event = new RateAirlineIntegrationEvent(userId, dto.Id, dto.Rate);

                try
                {
                    eventBus.Publish(@event);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to publish rateAirline event");
                    return StatusCode(500, "Failed to rate airline.");
                }

                return Ok();

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rate airline");
            }
        }
    }
}
