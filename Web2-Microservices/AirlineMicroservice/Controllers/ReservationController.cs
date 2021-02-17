using AirlineMicroservice.DTOs;
using AirlineMicroservice.IntegrationEvents.Events;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirlineMicroservice.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;
        private readonly IEventBus eventBus;

        public ReservationController(IUnitOfWork unitOfWork, HttpClient httpClient, IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.httpClient = httpClient;
            this.eventBus = eventBus;
        }
        [HttpPost]
        [Route("flight-reservation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> FlightReservation(FlightReservationDto dto)
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RegularUser"))
                {
                    return Unauthorized();
                }

                var result = (await httpClient.GetStringAsync("http://usermicroservice:80/api/user/user-json"));

                if (result == null)
                {
                    return NotFound();
                }

                dynamic userData = JObject.Parse(result);

                if (dto.MySeatsIds.Count == 0)
                {
                    return BadRequest("Choose at least one seat on flight");
                }

                var seatsForUpdate = new List<Seat>();

                var mySeats = await unitOfWork.SeatRepository.Get(s => dto.MySeatsIds.Contains(s.SeatId), null, "Flight");

                if (mySeats.ToList().Count != dto.MySeatsIds.Count)
                {
                    return BadRequest("Something went wrong");
                }

                var MyTickets = await unitOfWork.TicketRepository.Get(t => t.Reservation.UserId == userId, null, "Seat");

                if (MyTickets.FirstOrDefault(t => mySeats.Contains(t.Seat)) != null)
                {
                    return BadRequest("You already reserved seat on selected flight");
                }

                var err = mySeats.FirstOrDefault(s => s.Available == false || s.Reserved == true);

                if (err != null)
                {
                    return BadRequest("One of seats is already reserved by other user");
                }

                var myFlightReservation = new FlightReservation()
                {
                    UserId = userId,
                    ReservationDate = DateTime.Now,
                };

                var myTickets = new List<Ticket>();

                float totalPrice = 0;

                foreach (var seat in mySeats)
                {
                    myTickets.Add(new Ticket()
                    {
                        Seat = seat,
                        SeatId = seat.SeatId,
                        Reservation = myFlightReservation,
                        Price = seat.Price,
                        Passport = dto.MyPassport,
                    });

                    totalPrice += seat.Price;
                    seat.Available = false;
                    seat.Reserved = true;
                    seat.Ticket = myTickets.Last();
                    seatsForUpdate.Add(seat);
                }

                if (dto.WithBonus)
                {
                    if (totalPrice < userData.BonusPoints * 0.01)
                    {
                        totalPrice = 0;
                        userData.BonusPoints = (int)(userData.BonusPoints * 0.01 - totalPrice) * 100;
                    }
                    else
                    {
                        totalPrice = (float)(totalPrice - userData.BonusPoints * 0.01);
                        userData.BonusPoints = 0;
                    }
                }

                myFlightReservation.Price = totalPrice;
                myFlightReservation.Tickets = myTickets;

                //za neregistrovane prijatelje koji idu na putovanje******************

                var unregTicketList = new List<Ticket2>();

                foreach (var unregisteredRes in dto.UnregisteredFriends)
                {
                    var seat = await unitOfWork.SeatRepository.GetByID(unregisteredRes.SeatId);

                    if (seat == null)
                    {
                        return BadRequest("Seat not found");
                    }
                    if (!seat.Available || seat.Reserved)
                    {
                        return BadRequest("Selected seat is reserved already");
                    }

                    unregTicketList.Add(new Ticket2()
                    {
                        Seat = seat,
                        SeatId = seat.SeatId,
                        Reservation = myFlightReservation,
                        Price = seat.Price, //trebalo bi popust uracunati
                        Passport = unregisteredRes.Passport,
                        FirstName = unregisteredRes.FirstName,
                        LastName = unregisteredRes.LastName,
                    });

                    seat.Available = false;
                    seat.Reserved = true;
                    seat.Ticket2 = unregTicketList.Last();
                    seatsForUpdate.Add(seat);
                }

                myFlightReservation.UnregistredFriendsTickets = unregTicketList;

                //za registrovane prijatelje

                var invitationList = new List<Invitation>();

                foreach (var friend in dto.Friends)
                {
                    var seat = (await unitOfWork.SeatRepository.Get(s => s.SeatId == friend.SeatId, null, "Flight")).FirstOrDefault();

                    if (seat == null)
                    {
                        return BadRequest("Seat not found");
                    }
                    if (!seat.Available || seat.Reserved)
                    {
                        return BadRequest("Selected seat is reserved");
                    }

                    seat.Available = false;
                    seat.Reserved = true;
                    seatsForUpdate.Add(seat);
                    invitationList.Add(new Invitation()
                    {
                        SenderId = userId,
                        ReceiverId = friend.Id,
                        Seat = seat,
                        Price = seat.Price,
                        Expires = seat.Flight.TakeOffDateTime < DateTime.Now.AddDays(3) ?
                                    seat.Flight.TakeOffDateTime.AddHours(-3) : DateTime.Now.AddDays(3),
                    });
                }

                //CAR RESERVATION


                //saga transakcija

                try
                {
                    foreach (var seat in seatsForUpdate)
                    {
                        unitOfWork.SeatRepository.Update(seat);
                    }

                    await unitOfWork.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest("Somethin is modified in the meantime, or reserved by another user");
                }
                catch (Exception)
                {
                    return StatusCode(500, "One of transactions failed. Failed to reserve flight");
                }

                List<FlightInfo> userFlightInfo = new List<FlightInfo>();

                foreach (var res in myFlightReservation.Tickets)
                {
                    userFlightInfo.Add(new FlightInfo() { 
                        FlightNumber = res.Seat.Flight.FlightNumber,
                        From = res.Seat.Flight.From.City,
                        To = res.Seat.Flight.To.City,
                        Departure = res.Seat.Flight.TakeOffDateTime,
                        Arrival = res.Seat.Flight.LandingDateTime,
                        TripTime = res.Seat.Flight.TripTime,
                        TripLength = res.Seat.Flight.tripLength,
                        SeatClass = res.Seat.Class,
                        SeatRow = res.Seat.Row,
                        SeatColumn = res.Seat.Column,
                        TicketPrice = res.Price
                    });
                }

                TripInfo tripInfo = new TripInfo() {
                                TotalPrice = myFlightReservation.Price,
                                FlightInfo = userFlightInfo
                };

                try
                {
                    //send to user
                    var @event = new SendMailIntegrationEvent(userId, tripInfo);
                    eventBus.Publish(@event);
                }
                catch (Exception)
                {
                    Console.WriteLine("failed to send email");
                }

                foreach (var invitation in invitationList)
                {
                    FlightInfo fi = new FlightInfo()
                    {
                        FlightNumber = invitation.Seat.Flight.FlightNumber,
                        From = invitation.Seat.Flight.From.City,
                        To = invitation.Seat.Flight.To.City,
                        Departure = invitation.Seat.Flight.TakeOffDateTime,
                        Arrival = invitation.Seat.Flight.LandingDateTime,
                        TripTime = invitation.Seat.Flight.TripTime,
                        TripLength = invitation.Seat.Flight.tripLength,
                        SeatClass = invitation.Seat.Class,
                        SeatRow = invitation.Seat.Row,
                        SeatColumn = invitation.Seat.Column,
                        TicketPrice = invitation.Price
                    };

                    try
                    {
                        var @event = new SendMailToFriendIntegrationEvent(userId, invitation.ReceiverId, fi);
                        eventBus.Publish(@event);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("failed to send email");
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to reserve flight");
            }
        }


        [HttpDelete]
        [Route("cancel-flight-reservation/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CancelFlightReservation(int id)
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

                var reservation = await unitOfWork.FlightReservationRepository.GetReservationById(id);

                if (reservation == null)
                {
                    return NotFound();
                }

                if (reservation.UserId != userId)
                {
                    return BadRequest();
                }

                if (Math.Abs((reservation.Tickets.OrderBy(t => t.Seat.Flight.TakeOffDateTime).FirstOrDefault().Seat.Flight.TakeOffDateTime - DateTime.Now).TotalHours) < 3)
                {
                    return BadRequest("Cant cancel flight");
                }

                var listOfSeatsToUpdate = new List<Seat>();

                foreach (var ticket in reservation.Tickets)
                {
                    ticket.Seat.Available = true;
                    ticket.Seat.Reserved = false;
                    listOfSeatsToUpdate.Add(ticket.Seat);
                }

                //user.FlightReservations.Remove(reservation);

                //CarSpecialOffer carSpecOffer = null;

                //if (reservation.CarRent != null)
                //{
                //    var specialOffers = await unitOfWork.RACSSpecialOfferRepository.Get(s => s.Car == reservation.CarRent.RentedCar
                //                                    && s.FromDate == reservation.CarRent.TakeOverDate && s.ToDate == reservation.CarRent.ReturnDate);
                //    carSpecOffer = specialOffers.FirstOrDefault();

                //    if (carSpecOffer != null)
                //    {
                //        carSpecOffer.IsReserved = false;
                //    }
                //    user.CarRents.Remove(reservation.CarRent);
                //    reservation.CarRent.RentedCar.Rents.Remove(reservation.CarRent);
                //}

                //try
                //{
                //    foreach (var seat in listOfSeatsToUpdate)
                //    {
                //        unitOfWork.SeatRepository.Update(seat);
                //    }

                //    unitOfWork.UserRepository.Update(user);
                //    if (reservation.CarRent != null)
                //    {
                //        unitOfWork.CarRepository.Update(reservation.CarRent.RentedCar);
                //    }
                //    if (carSpecOffer != null)
                //    {
                //        unitOfWork.RACSSpecialOfferRepository.Update(carSpecOffer);
                //    }

                //    await unitOfWork.Commit();
                //}
                //catch (Exception)
                //{
                //    return StatusCode(500, "Failed to cancel reservation");
                //}

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to cancel reservation");
            }
        }

        //[HttpPost]
        //[Route("reserve-special-offer-flight")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> ReserveSpecialOfferFlight([FromBody] ReserveFlightDto dto)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //        var user = (User)await unitOfWork.UserManager.FindByIdAsync(userId);

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("RegularUser"))
        //        {
        //            return Unauthorized();
        //        }

        //        if (user == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        var specialOffer = await unitOfWork.SpecialOfferRepository.GetSpecialOfferById(dto.Id);

        //        if (specialOffer == null)
        //        {
        //            return NotFound("Selected special offer is not found");
        //        }

        //        if (specialOffer.IsReserved)
        //        {
        //            return BadRequest("Already reserved");
        //        }

        //        var flightReservation = new FlightReservation()
        //        {
        //            User = user,
        //            Price = specialOffer.NewPrice,
        //            ReservationDate = DateTime.Now
        //        };

        //        var tickets = new List<Ticket>();

        //        foreach (var seat in specialOffer.Seats)
        //        {
        //            if (seat.Flight.TakeOffDateTime < DateTime.Now)
        //            {
        //                return BadRequest("One of flights is in past");
        //            }

        //            tickets.Add(new Ticket()
        //            {
        //                Seat = seat,
        //                SeatId = seat.SeatId,
        //                Reservation = flightReservation,
        //                Passport = dto.Passport
        //            });
        //        }

        //        flightReservation.Tickets = tickets;

        //        user.FlightReservations.Add(flightReservation);
        //        specialOffer.IsReserved = true;

        //        try
        //        {
        //            unitOfWork.SpecialOfferRepository.Update(specialOffer);
        //            unitOfWork.UserRepository.Update(user);

        //            await unitOfWork.Commit();
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return BadRequest("Something is modified in the meantime, or reserved by another user");
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Failed to reserve special offer");
        //        }

        //        try
        //        {
        //            await unitOfWork.AuthenticationRepository.SendTicketConfirmationMail(user, flightReservation);
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "Successfully reserved, but failed to send email");
        //        }

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Failed to reserve special offer");
        //    }
        //}


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
