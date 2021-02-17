using AirlineMicroservice.DTOs;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        public AirlineController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            this.unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
        }

        

        [HttpPost]
        [Route("create-airline")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateAirline([FromBody] AirlineInfoDto registerDto) 
        {
            try
            {

                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("SystemAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var airline = new Airline()
                {
                    Name = registerDto.Name,
                    Address = new Address()
                    {
                        City = registerDto.City,
                        State = registerDto.State,
                        Lat = registerDto.Lat,
                        Lon = registerDto.Lon
                    },
                    AdminId = registerDto.AdminId
                };

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to create airline");
            }
            
        }

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

                HttpStatusCode sc = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (sc.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
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

                var result = IdentityResult.Success;
                if (addressChanged)
                {
                    try
                    {
                        unitOfWork.AirlineRepository.Update(airline);
                        unitOfWork.AirlineRepository.UpdateAddress(airline.Address);

                        await unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Failed to change airline info. One of transactions failed.");
                    }
                }
                else
                {
                    try
                    {
                        unitOfWork.AirlineRepository.Update(airline);
                        await unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Failed to change airline info. Transaction failed.");
                    }
                }

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception)
            {
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
                    return NotFound();
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

                try
                {
                    unitOfWork.AirlineRepository.Update(airline);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to change. Transaction failed.");
                }

                return Ok();
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

                try
                {
                    await unitOfWork.SpecialOfferRepository.Insert(specialOffer);
                    unitOfWork.AirlineRepository.Update(airline);

                    foreach (var seat in seats)
                    {
                        seat.Available = false;
                        unitOfWork.SeatRepository.Update(seat);
                    }

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to add special offer. One of transactions failed.");
                }

                return Ok();
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

                try
                {
                    unitOfWork.SpecialOfferRepository.Delete(specOffer);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to delete special offer");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete special offer");
            }
        }

        #endregion
    }
}
