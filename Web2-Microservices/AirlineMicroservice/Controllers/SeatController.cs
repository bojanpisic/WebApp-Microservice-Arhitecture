using AirlineMicroservice.DTOs;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public SeatController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
            httpClient = new HttpClient();
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
                try
                {
                    unitOfWork.SeatRepository.Delete(seat);
                    await unitOfWork.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Something is changed. Cant delete");
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to delete seat. Transaction failed.");
                }

                return Ok();
                //var seats = await unitOfWork.SeatRepository.Get(s => s.Flight == seat.Flight);

                //List<object> obj = new List<object>();

                //foreach (var item in seats)
                //{
                //    obj.Add(new { item.Column, item.Row, item.Flight.FlightId, item.Class, item.SeatId, item.Price });
                //}
                //return Ok(obj);
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

                try
                {
                    await unitOfWork.SeatRepository.Insert(seat);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    //unitOfWork.Rollback();
                    return StatusCode(500, "Failed to add seat. Transaction failed.");
                }

                return Ok();
                //var allSeats = await unitOfWork.SeatRepository.Get(s => s.Flight == flight);
                //List<object> obj = new List<object>();

                //foreach (var item in allSeats)
                //{
                //    obj.Add(new { item.Column, item.Row, item.Flight.FlightId, item.Class, item.SeatId, item.Price });
                //}

                //return Ok(obj);
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

                try
                {
                    unitOfWork.SeatRepository.Update(seat);
                    await unitOfWork.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Something is changed. Cant change");
                }
                catch (Exception)
                {
                    //unitOfWork.Rollback();
                    return StatusCode(500, "Failed to update seat. Transaction failed.");
                }

                return Ok();
                //var seats = await unitOfWork.SeatRepository.Get(s => s.Flight == seat.Flight);

                //List<object> obj = new List<object>();

                //foreach (var item in seats)
                //{
                //    obj.Add(new { item.Column, item.Row, item.Flight.FlightId, item.Class, item.SeatId, item.Price });
                //}

                //return Ok(obj);
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

        #endregion
    }
}
