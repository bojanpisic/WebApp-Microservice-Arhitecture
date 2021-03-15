using AirlineMicroservice.DTOs;
using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirlineMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;
        private readonly IEventBus eventBus;

        public FlightController(IUnitOfWork _unitOfWork, HttpClient httpClient, IEventBus eventBus)
        {
            this.unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
            this.eventBus = eventBus;
        }

        [HttpGet]
        [Route("test1")]
        public async Task<IActionResult> test1()
        {
            HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}","1232332"))).StatusCode;

            if (result == HttpStatusCode.OK)
            {
                Console.WriteLine("nadjen");
            }
            else 
            {
                Console.WriteLine("ne");
            }

            return Ok(result.ToString());
        }

        [HttpGet]
        [Route("test2")]
        public async Task<IActionResult> test2()
        {
            var result = (await httpClient.GetStringAsync("http://usermicroservice:80/api/user/test"));
            dynamic data = JObject.Parse(result);
            Console.WriteLine(data.id.ToString());
            Console.WriteLine(data.firstName);

            return Ok(result.ToString());
        }

        #region Flight methods

        [HttpPost]
        [Route("add-flight")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddFlight(FlightDto flightDto)
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

                var airline = (await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId)).FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                var exists = (await unitOfWork.FlightRepository.Get(f => f.Airline == airline &&
                                                                    f.FlightNumber == flightDto.FlightNumber)).FirstOrDefault();

                //proveri da li postoji vec takav naziv leta
                if (exists != null)
                {
                    return BadRequest("Flight num already exist");
                }

                var day = Convert.ToDateTime(flightDto.TakeOffDateTime).Day;

                if (Convert.ToDateTime(flightDto.TakeOffDateTime) > Convert.ToDateTime(flightDto.LandingDateTime)
                    || Convert.ToDateTime(flightDto.TakeOffDateTime) < DateTime.Now)
                {
                    return BadRequest("Take of time has to be lower then landing time");
                }

                var fromIdDest = await unitOfWork.DestinationRepository.GetByID(flightDto.FromId);
                var toIdDest = await unitOfWork.DestinationRepository.GetByID(flightDto.ToId);

                if (fromIdDest == null || toIdDest == null)
                {
                    return BadRequest("Destination is not on airline");
                }

                var stops = (await unitOfWork.DestinationRepository.GetAirlineDestinations(airline))
                                   .Where(s => flightDto.StopIds.Contains(s.DestinationId));

                var flight = new Flight()
                {
                    FlightNumber = flightDto.FlightNumber,
                    TakeOffDateTime = Convert.ToDateTime(flightDto.TakeOffDateTime),
                    LandingDateTime = Convert.ToDateTime(flightDto.LandingDateTime),
                    From = fromIdDest,
                    To = toIdDest,
                    Airline = airline,
                    tripLength = flightDto.TripLength
                };
                var tripTime = await GetFlightTime(flight.From.City, flight.From.State, flight.To.City, flight.To.City,
                                                              flight.TakeOffDateTime, flight.LandingDateTime);
                flight.TripTime = tripTime;

                flight.Seats = new HashSet<Seat>();

                foreach (var seat in flightDto.Seats)
                {
                    flight.Seats.Add(new Seat()
                    {
                        Column = seat.Column,
                        Row = seat.Row,
                        Class = seat.Class,
                        Price = seat.Price,
                        Flight = flight,
                        Available = true,
                        Reserved = false,
                        Ticket = null
                    });
                }

                foreach (var stop in stops)
                {
                    flight.Stops = new List<FlightDestination>
                    {
                        new FlightDestination{
                            Flight = flight,
                            Destination = stop
                        }
                    };
                }

                await unitOfWork.FlightRepository.Insert(flight);
                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Insert flight failed");
                return StatusCode(500, "Failed to add flight.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add flight.");
            }

        }

        [HttpGet]
        [Route("get-flights")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult<IEnumerable<Flight>>> GetFlightsOfAirline()
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

                var findRes = await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId);

                var airline = findRes.FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                var flights = await unitOfWork.FlightRepository.GetAirlineFlights(airline.AirlineId); // ne moze get jer treba uvesti i stops -> destinations

                ICollection<object> flightsObject = new List<object>();

                foreach (var flight in flights)
                {
                    List<object> stops = new List<object>();

                    if (flight.Stops != null)
                    {
                        foreach (var stop in flight.Stops)
                        {
                            stops.Add(new
                            {
                                stop.Destination.City
                            });
                        }
                    }


                    flightsObject.Add(new
                    {
                        takeOffDate = flight.TakeOffDateTime.Date,
                        landingDate = flight.LandingDateTime.Date,
                        airlineLogo = airline.LogoUrl,
                        airlineName = airline.Name,
                        from = flight.From.City,
                        to = flight.To.City,
                        takeOffTime = flight.TakeOffDateTime.TimeOfDay,
                        landingTime = flight.LandingDateTime.TimeOfDay,
                        flightTime = flight.TripTime,
                        flightLength = flight.tripLength,
                        flightNumber = flight.FlightNumber,
                        flightId = flight.FlightId,
                        stops = stops
                    });
                }

                return Ok(flightsObject);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return flights.");
            }
        }

        [HttpPost]
        [Route("rate-flight")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RateFlight(RateDto dto)
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


                var flights = await unitOfWork.FlightRepository.Get(f => f.FlightId == dto.Id, null, "Rates");

                var flight = flights.FirstOrDefault();

                if (flight == null)
                {
                    return BadRequest("Flight not found");
                }

                if (flight.Rates.FirstOrDefault(r => r.UserId == userId) != null)
                {
                    return BadRequest("You already rate this flight");
                }

                //PROVERA DA LI JE LETIO NA OVOM LETU
                var flightReservations = await unitOfWork.FlightReservationRepository
                                                .Get(f => f.Tickets.FirstOrDefault(t => t.Seat.Flight == flight) != null && f.UserId == userId,
                                                null,
                                                "Tickets");

                var flightReservation = flightReservations.FirstOrDefault();

                if (flightReservation == null)
                {
                    return BadRequest("You didnt reserve seat on this flight. Cant rate.");
                }

                var ticketOfReservation = flightReservation.Tickets.FirstOrDefault();
                if (ticketOfReservation == null)
                {
                    return BadRequest();
                }

                var ticket = await unitOfWork.TicketRepository.GetTicket(ticketOfReservation.TicketId);

                if (ticket == null)
                {
                    return BadRequest();
                }

                if (ticket.Seat.Flight.LandingDateTime >= DateTime.Now)
                {
                    return BadRequest("You can rate flight after landing");
                }

                flight.Rates.Add(new FlightRate()
                {
                    Rate = dto.Rate,
                    UserId = userId,
                    Flight = flight
                });

                unitOfWork.FlightRepository.Update(flight);

                await unitOfWork.Commit();


                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rate flight");
            }
        }

        #endregion

        private async Task<string> GetFlightTime(string departureCity, string depState, string arrivalCity, string arrState, DateTime departureDate, DateTime arrivalDate)
        {
            await Task.Yield();

            var timeZoneInfoDeparture = TimeZoneInfo.GetSystemTimeZones()
                        .Where(k => k.DisplayName.Substring(k.DisplayName.IndexOf(')') + 2).ToLower().IndexOf(departureCity.ToLower()) >= 0)
                        .ToList();
            var timeZoneInfoLanding = TimeZoneInfo.GetSystemTimeZones()
            .Where(k => k.DisplayName.Substring(k.DisplayName.IndexOf(')') + 2).ToLower().IndexOf(arrivalCity.ToLower()) >= 0)
            .ToList();

            if (timeZoneInfoDeparture.Count == 0)
            {
                timeZoneInfoDeparture = TimeZoneInfo.GetSystemTimeZones()
                       .Where(k => k.DisplayName.Substring(k.DisplayName.IndexOf(')') + 2).ToLower().IndexOf(depState.ToLower()) >= 0)
                       .ToList();
            }
            if (timeZoneInfoLanding.Count == 0)
            {
                timeZoneInfoLanding = TimeZoneInfo.GetSystemTimeZones()
                .Where(k => k.DisplayName.Substring(k.DisplayName.IndexOf(')') + 2).ToLower().IndexOf(arrState.ToLower()) >= 0)
                .ToList();
            }

            if (timeZoneInfoDeparture.Count == 0 || timeZoneInfoLanding.Count == 0)
            {
                var hourr = Math.Abs(arrivalDate.Hour - (departureDate.Hour));
                var minutess = Math.Abs(departureDate.Minute - arrivalDate.Minute);

                string flightTimee = hourr + "h " + minutess + "min";

                return flightTimee;
            }

            int departureZone = (int)timeZoneInfoDeparture[0].BaseUtcOffset.TotalHours;
            int landingZone = (int)timeZoneInfoLanding[0].BaseUtcOffset.TotalHours;
            //int year, int month, int day, int hour, int minute, int second

            int hour;
            if (departureZone >= landingZone)
            {
                hour = Math.Abs(arrivalDate.Hour - (departureDate.Hour - (departureZone - landingZone)));
            }
            else // (departureZone < landingZone)
            {
                hour = Math.Abs(arrivalDate.Hour - (departureDate.Hour + (landingZone - departureZone)));
            }
            var minutes = 0;
            if (arrivalDate.Minute >= departureDate.Minute)
            {
                minutes = Math.Abs(departureDate.Minute - arrivalDate.Minute);
            }
            else
            {
                minutes = 60 - departureDate.Minute - arrivalDate.Minute;
            }

            string flightTime = hour + "h " + minutes + "min";

            return flightTime;
        }

    }
}
