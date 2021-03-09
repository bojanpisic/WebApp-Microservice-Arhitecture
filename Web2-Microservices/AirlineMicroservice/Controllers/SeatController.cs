using AirlineMicroservice.DTOs;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class SeatController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public SeatController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            this.unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
        }
        #region Seat methods

        [HttpDelete]
        [Route("delete-seat/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> DeleteSeat(int id)
        {
            try
            {
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("AirlineAdmin"))
                {
                    return Unauthorized();
                }

                var seat = (await unitOfWork.SeatRepository.Get(s => s.SeatId == id, null, "Flight")).FirstOrDefault();

                if (seat == null)
                {
                    return NotFound("Seat not found");
                }

                if (!seat.Available || seat.Reserved)
                {
                    return BadRequest("Cant delete seat. Seat is reserved");
                }

                unitOfWork.SeatRepository.Delete(seat);
                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Something is changed. Cant delete");
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("failed to delete seat. Transaction failed");
                return StatusCode(500, "Failed to delete seat. Transaction failed.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete seat.");
            }

        }

        [HttpPost]
        [Route("add-seat")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddSeat(SeatDto seatDto)
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

                if (seatDto.Price < 0)
                {
                    return BadRequest("Price input is wrong");
                }

                var res = await unitOfWork.FlightRepository.Get(f => f.FlightId == seatDto.FlightId);
                var flight = res.FirstOrDefault();

                if (flight == null)
                {
                    return NotFound("Flight not found");
                }

                var seat = new Seat()
                {
                    Column = seatDto.Column,
                    Row = seatDto.Row,
                    Price = seatDto.Price,
                    Class = seatDto.Class,
                    Reserved = false,
                    Available = true,
                    Flight = flight
                };


                await unitOfWork.SeatRepository.Insert(seat);
                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("transaction failed");
                return StatusCode(500, "Failed to add seat.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add seat.");
            }
        }

        //[Authorize(Roles = "AirlineAdmin")]
        [HttpPut]
        [Route("change-seat/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeSeatPrice(int id, ChangeSeatDto seatDto)
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

                //var user = await unitOfWork.UserManager.FindByIdAsync(userId);

                //if (user == null)
                //{
                //    return NotFound("User not found");
                //}

                if (seatDto.Price < 0)
                {
                    return BadRequest("Price input is wrong");
                }

                var seat = await unitOfWork.SeatRepository.GetByID(id);

                if (seat == null)
                {
                    return NotFound("Seat not found.");
                }

                if (!seat.Available || seat.Reserved)
                {
                    return BadRequest("Cant change seat. Seat is reserved");
                }

                seat.Price = seatDto.Price;


                unitOfWork.SeatRepository.Update(seat);
                await unitOfWork.Commit();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Something is changed. Cant change");
            }
            catch(DbUpdateException)
            {
                Console.WriteLine("Failed to update seat. Transaction failed");
                return StatusCode(500, "Failed to update seat.");

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to update seat.");
            }
        }

        [HttpGet]
        [Route("get-seats/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetSeats(int id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
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

                var res = await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId); //getairlinebyadmin
                var airline = res.FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found.");
                }

                var flight = await unitOfWork.FlightRepository.GetByID(id);

                if (flight == null)
                {
                    return NotFound("Flight not found.");
                }

                var seats = await unitOfWork.SeatRepository.Get(s => s.Flight == flight);

                List<object> obj = new List<object>();

                foreach (var item in seats)
                {
                    obj.Add(new
                    {
                        item.Column,
                        item.Row,
                        item.Flight.FlightId,
                        item.Class,
                        item.SeatId,
                        item.Price
                    });
                }

                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return seats.");
            }

        }

        [HttpGet]
        [Route("get-seat")]
        
        public async Task<IActionResult> GetSeat(int id) 
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return null;
            }

            try
            {
                var seat = await unitOfWork.SeatRepository.GetSeat(id);

                var obj = new {
                    Column = seat.Column,
                    Row = seat.Row,
                    FlightId = seat.Flight.FlightId,
                    Class1 = seat.Class,
                    SeatId = seat.SeatId,
                    Price = seat.Price,
                    TakeOffDate = seat.Flight.TakeOffDateTime.Date,
                    LandingDate = seat.Flight.LandingDateTime.Date,
                    AirlineLogo = seat.Flight.Airline.LogoUrl,
                    AirlineName = seat.Flight.Airline.Name,
                    AirlineId =seat.Flight.Airline.AirlineId,
                    From = seat.Flight.From.City,
                    To = seat.Flight.To.City,
                    TakeOffTime = seat.Flight.TakeOffDateTime.TimeOfDay,
                    LandingTime = seat.Flight.TakeOffDateTime.TimeOfDay,
                    FlightTime = seat.Flight.TripTime,
                    FlightLength =seat.Flight.tripLength,
                    FlightNumber = seat.Flight.FlightNumber,
                };

                return Ok(obj);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
