using AirlineMicroservice.DTOs;
using AirlineMicroservice.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class TripController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public TripController(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            this.unitOfWork = unitOfWork;
            this.httpClient = httpClient;
        }
        [HttpPost]
        [Route("get-trip-info")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTripInfo([FromBody] InfoDto dto)
        {
            try
            {

                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                //dobavi usera
                var result = await httpClient.GetStringAsync(String.Format("http://usermicroservice:80/api/user/find-userbyid?id={0}", userId));
                dynamic data = JObject.Parse(result);

                var mySeats = await unitOfWork.SeatRepository.Get(s => dto.MySeatsIds.Contains(s.SeatId));
                var seats = new List<object>();
                foreach (var item in mySeats)
                {
                    seats.Add(new
                    {
                        row = item.Row,
                        clas = item.Class,
                        col = item.Column,
                    });
                }

                var friendList = new List<object>();

                foreach (var item in dto.Friends)
                {
                    var result1 = await httpClient.GetStringAsync(String.Format("http://usermicroservice:80/api/user/find-userbyid?id={0}", userId));
                    dynamic data1 = JObject.Parse(result1);
                    //var friend = await unitOfWork.UserRepository.GetByID(item.Id);
                    var seat = await unitOfWork.SeatRepository.GetByID(item.SeatId);
                    friendList.Add(new
                    {
                        friendEmail = data1.email,
                        friendFirstName = data1.firstName,
                        friendLastName = data1.lastName,
                        column = seat.Column,
                        row = seat.Row,
                        clas = seat.Class,
                    });
                }
                var unregisteredFriendList = new List<object>();

                foreach (var item in dto.UnregisteredFriends)
                {
                    var seat = await unitOfWork.SeatRepository.GetByID(item.SeatId);
                    unregisteredFriendList.Add(new
                    {
                        lastName = item.LastName,
                        firstName = item.FirstName,
                        passport = item.Passport,
                        column = seat.Column,
                        row = seat.Row,
                        clas = seat.Class,
                    });
                }
                float totalPrice = 0;

                foreach (var item in mySeats)
                {
                    totalPrice += item.Price;
                }

                float priceWithBonus = 0;
                Console.WriteLine(data.bonusPoints);

                if (totalPrice < (float)data.bonusPoints * 0.01)
                {
                    priceWithBonus = 0;
                }
                else
                {
                    priceWithBonus = (float)(totalPrice - (float)data.bonusPoints * 0.01);
                }

                return Ok(new
                {
                    priceWithBonus = priceWithBonus,
                    totalPrice = totalPrice,
                    friends = friendList,
                    unregisteredFriends = unregisteredFriendList,
                    mySeats = seats,
                    myBonus = data.bonusPoints
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Failed to return trip info");
            }
        }


        [HttpGet]
        [Route("get-previous-flights")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPreviousFlights()
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

                var trips = await unitOfWork.FlightReservationRepository.GetTrips(userId);
                var retVal = new List<object>();

                foreach (var trip in trips)
                {
                    foreach (var ticket in trip.Tickets)
                    {
                        if (ticket.Seat.Flight.LandingDateTime >= DateTime.Now)
                        {
                            continue;
                        }
                        List<object> stops = new List<object>();

                        foreach (var stop in ticket.Seat.Flight.Stops)
                        {
                            stops.Add(new
                            {
                                stop.Destination.City,
                                stop.Destination.State
                            });
                        }

                        retVal.Add(new
                        {
                            column = ticket.Seat.Column,
                            row = ticket.Seat.Row,
                            clas = ticket.Seat.Class,
                            seatId = ticket.Seat.SeatId,
                            seatPrice = ticket.Seat.Price,
                            takeOffDate = ticket.Seat.Flight.TakeOffDateTime.Date,
                            landingDate = ticket.Seat.Flight.LandingDateTime.Date,
                            airlineLogo = ticket.Seat.Flight.Airline.LogoUrl,
                            airlineName = ticket.Seat.Flight.Airline.Name,
                            airlineId = ticket.Seat.Flight.Airline.AirlineId,
                            from = ticket.Seat.Flight.From.City,
                            to = ticket.Seat.Flight.To.City,
                            takeOffTime = ticket.Seat.Flight.TakeOffDateTime.TimeOfDay,
                            landingTime = ticket.Seat.Flight.LandingDateTime.TimeOfDay,
                            flightTime = ticket.Seat.Flight.TripTime,
                            flightLength = ticket.Seat.Flight.tripLength,
                            flightNumber = ticket.Seat.Flight.FlightNumber,
                            flightId = ticket.Seat.Flight.FlightId,
                            stops = stops,
                            isAirlineRated = ticket.Seat.Flight.Airline.Rates.FirstOrDefault(r => r.UserId == userId) == null ?
                                             false : true,
                            isFlightRated = ticket.Seat.Flight.Rates.FirstOrDefault(r => r.UserId == userId) == null ?
                                             false : true,
                        });
                    }
                }

                return Ok(retVal);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return trips");
            }
        }

        [HttpGet]
        [Route("get-upcoming-trips")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUpcomingTrips()
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

                var trips = await unitOfWork.FlightReservationRepository.GetTrips(userId);
                var flights = new List<object>();
                var retVal = new List<object>();

                foreach (var trip in trips)
                {
                    var previous = trip.Tickets.Where(t => t.Seat.Flight.LandingDateTime < DateTime.Now);
                    if (previous.ToList().Count == trip.Tickets.ToList().Count)
                    {
                        continue;
                    }

                    flights = new List<object>();

                    foreach (var ticket in trip.Tickets)
                    {

                        List<object> stops = new List<object>();

                        foreach (var stop in ticket.Seat.Flight.Stops)
                        {
                            stops.Add(new
                            {
                                stop.Destination.City,
                                stop.Destination.State
                            });
                        }

                        flights.Add(new
                        {
                            column = ticket.Seat.Column,
                            row = ticket.Seat.Row,
                            clas = ticket.Seat.Class,
                            seatId = ticket.Seat.SeatId,
                            seatPrice = ticket.Seat.Price,
                            takeOffDate = ticket.Seat.Flight.TakeOffDateTime.Date,
                            landingDate = ticket.Seat.Flight.LandingDateTime.Date,
                            airlineLogo = ticket.Seat.Flight.Airline.LogoUrl,
                            airlineName = ticket.Seat.Flight.Airline.Name,
                            airlineId = ticket.Seat.Flight.Airline.AirlineId,
                            from = ticket.Seat.Flight.From.City,
                            to = ticket.Seat.Flight.To.City,
                            takeOffTime = ticket.Seat.Flight.TakeOffDateTime.TimeOfDay,
                            landingTime = ticket.Seat.Flight.LandingDateTime.TimeOfDay,
                            flightTime = ticket.Seat.Flight.TripTime,
                            flightLength = ticket.Seat.Flight.tripLength,
                            flightNumber = ticket.Seat.Flight.FlightNumber,
                            flightId = ticket.Seat.Flight.FlightId,
                            stops = stops,
                            canCancel = Math.Abs((ticket.Seat.Flight.TakeOffDateTime - DateTime.Now).TotalHours) >= 3,
                        });
                    }
                    retVal.Add(new
                    {
                        reservationId = trip.FlightReservationId,
                        flights = flights,
                        totalPrice = trip.Price
                    });
                }

                return Ok(retVal);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return trips");
            }
        }
    }
}
