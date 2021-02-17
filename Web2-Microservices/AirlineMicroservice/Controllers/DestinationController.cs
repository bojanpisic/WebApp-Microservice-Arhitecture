using AirlineMicroservice.DTOs;
using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class DestinationController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public DestinationController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            this.unitOfWork = _unitOfWork;
        }

        #region Destination methods
        [HttpGet]
        [Route("get-airline-destinations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAirlineDestinations()
        {
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


                var airline = (await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId)).FirstOrDefault();
                //var res = await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId, null, "Flights,Address");

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                var result = await unitOfWork.DestinationRepository.GetAirlineDestinations(airline);

                List<object> obj = new List<object>();

                foreach (var item in result)
                {
                    obj.Add(new
                    {
                        city = item.City,
                        state = item.State,
                        destinationId = item.DestinationId,
                        imageUrl = item.ImageUrl
                    });
                }
                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return destinations.");
            }
        }
        [HttpPost]
        [Route("add-destination")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddDestination(DestinationDto destinationDto)
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

                var res = await unitOfWork.AirlineRepository.Get(a => a.AdminId == userId);
                var airline = res.FirstOrDefault();

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                var airlineDestinations = await unitOfWork.DestinationRepository.GetAirlineDestinations(airline);

                if (airlineDestinations.FirstOrDefault(d => d.City == destinationDto.City
                                                        && d.State == destinationDto.State) != null)
                {
                    return BadRequest("Airline already has selected destination");
                }


                byte[] imageBytes;
                using (var webClient = new WebClient())
                {
                    imageBytes = webClient.DownloadData(destinationDto.ImgUrl);
                }

                var destination = new Destination()
                {
                    City = destinationDto.City,
                    State = destinationDto.State,
                    ImageUrl = imageBytes
                };

                destination.Airlines = new List<AirlineDestination>
                {
                    new AirlineDestination
                    {
                        Destination = destination,
                        Airline = airline,
                    }
                };
                try
                {
                    await unitOfWork.DestinationRepository.Insert(destination);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to add destination.");
                }

                return Ok();
                //var allDestinations = await unitOfWork.DestinationRepository.GetAirlineDestinations(airline);

                //List<object> obj = new List<object>();

                //foreach (var item in allDestinations)
                //{
                //    obj.Add(new
                //    {
                //        city = item.City,
                //        state = item.State,
                //        destinationId = item.DestinationId,
                //        imageUrl = item.ImageUrl
                //    });
                //}
                //return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add destination.");
            }

        }

        //[Authorize(Roles = "AirlineAdmin")]

        //[HttpDelete]
        //[Route("delete-destination/{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        //public async Task<IActionResult> DeleteDestination(int id)
        //{
        //    try
        //    {
        //        string userId = User.Claims.First(c => c.Type == "UserID").Value;

        //        string userRole = User.Claims.First(c => c.Type == "Roles").Value;

        //        if (!userRole.Equals("AirlineAdmin"))
        //        {
        //            return Unauthorized();
        //        }

        //        var user = await unitOfWork.UserManager.FindByIdAsync(userId);

        //        if (user == null)
        //        {
        //            return NotFound(new IdentityError() { Code = "400", Description = "User not found" });
        //        }

        //        var destination = await unitOfWork.AirlineRepository.GetDestination(id);

        //        if (destination == null)
        //        {
        //            return BadRequest(new IdentityError() { Code = "Destination not found" });
        //        }

        //        var airline = await unitOfWork.AirlineRepository.GetAirlineByAdmin(userId);

        //        if (airline == null)
        //        {
        //            return NotFound(new IdentityError() { Code = "404", Description = "Airline not found" });
        //        }

        //        airline.Destinations.Remove(destination);

        //        var result = await unitOfWork.AirlineRepository.UpdateArline(airline);

        //        if (result.Succeeded)
        //        {
        //            var airline = await unitOfWork.AirlineRepository.GetAirlineByAdmin(userId);
        //            var dest = await unitOfWork.AirlineRepository.GetAirlineDestinations(airline);

        //            List<object> obj = new List<object>();

        //            foreach (var item in dest)
        //            {
        //                obj.Add(new 
        //                {
        //                    city = item.City,
        //                    state = item.State,
        //                    destinationId = item.DestinationId,
        //                    imageUrl = item.ImageUrl
        //                });
        //            }
        //            return Ok(obj);
        //        }

        //        return BadRequest(result.Errors);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Internal Server error");
        //    }

        //}

        #endregion

    }
}
