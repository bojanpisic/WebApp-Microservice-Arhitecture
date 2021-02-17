using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RACSMicroservice.DTOs;
using RACSMicroservice.Models;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RACSMicroservice.Controllers
{
    public class RentACarController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public RentACarController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
        }

        [HttpGet]
        [Route("get-racs-address")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRACSAddress()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId, null, "Address");
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                object obj = new
                {
                    city = racs.Address.City,
                    state = racs.Address.State
                };

                return Ok(obj);

            }
            catch (Exception)
            {

                return StatusCode(500, "Failed to return RACS address");
            }
        }

        [HttpGet]
        [Route("get-racs")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetRACSProfile()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }
                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId, null, "Address");
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                var sum = 0.0;
                foreach (var r in racs.Rates)
                {
                    sum += r.Rate;
                }

                float rate = sum == 0 ? 0 : (float)sum / racs.Rates.ToArray().Length;

                object obj = new
                {
                    racs.Name,
                    racs.About,
                    racs.Address,
                    racs.LogoUrl,
                    rate = rate
                };

                return Ok(obj);

            }
            catch (Exception)
            {

                return StatusCode(500, "Failed to return profile");
            }

        }

        #region Special offer methods
        [HttpPost]
        [Route("add-car-specialoffer/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddSpecialOffer([FromBody] CarSpecialOfferDto specialOfferDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
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

                var ress = await unitOfWork.CarRepository.Get(c => c.CarId == id, null, "SpecialOffers,Rents");
                var car = ress.FirstOrDefault();

                if (car == null)
                {
                    return NotFound("Car not found");
                }

                //var oldSpecOffers = await unitOfWork.RentACarRepository.GetSpecialOffersOfCar(car);
                var oldSpecOffers = car.SpecialOffers;

                var specialFromDate = Convert.ToDateTime(specialOfferDto.FromDate);
                var specialToDate = Convert.ToDateTime(specialOfferDto.ToDate);

                if (specialFromDate < DateTime.Now.Date)
                {
                    return BadRequest("Date is in past");
                }

                if (specialFromDate > specialToDate)
                {
                    return BadRequest("From date should be lower then to date");
                }

                foreach (var item in oldSpecOffers)
                {
                    if (item.FromDate <= specialFromDate && item.ToDate >= specialToDate
                        || item.FromDate >= specialFromDate && item.ToDate <= specialToDate
                        || item.FromDate <= specialFromDate && item.ToDate >= specialFromDate
                        || item.FromDate <= specialToDate && item.ToDate >= specialToDate)
                    {
                        return BadRequest("Dates are unavailable. Car has another special offers in that time.");
                    }
                }

                foreach (var item in car.Rents)
                {
                    if (item.TakeOverDate <= specialFromDate && item.ReturnDate >= specialToDate
                        || item.TakeOverDate >= specialFromDate && item.ReturnDate <= specialToDate
                        || item.TakeOverDate <= specialFromDate && item.ReturnDate >= specialFromDate
                        || item.TakeOverDate <= specialToDate && item.ReturnDate >= specialToDate)
                    {
                        return BadRequest("Dates are unavailable. Car has reservation in that time.");
                    }
                }


                var specialOffer = new CarSpecialOffer()
                {
                    Car = car,
                    NewPrice = specialOfferDto.NewPrice,
                    FromDate = specialFromDate,
                    ToDate = specialToDate,
                    OldPrice = car.PricePerDay * (float)(specialToDate - specialFromDate).TotalDays
                };

                //var oldPrice = Math.Abs(specialFromDate.Day - specialToDate.Day + 1) * car.PricePerDay;

                //specialOffer.OldPrice = oldPrice;

                car.SpecialOffers.Add(specialOffer);

                //using (var transaction = new TransactionScope())
                //{
                try
                {
                    await unitOfWork.RACSSpecialOfferRepository.Insert(specialOffer);
                    //unitOfWork.Commit();

                    unitOfWork.CarRepository.Update(car);
                    await unitOfWork.Commit();

                    //await transaction.Result.CommitAsync();
                    //transaction.Complete();
                }
                catch (Exception)
                {
                    //unitOfWork.Rollback();
                    //transaction.Dispose();
                    //await transaction.Result.RollbackAsync();
                    return StatusCode(500, "Failed to add special offer");
                }
                //}
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add special offer");
            }
        }

        [HttpGet]
        [Route("get-cars-specialoffers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetSpecialOffers()
        {
            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;

                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId);
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                var specOffers = await unitOfWork.RACSSpecialOfferRepository.GetSpecialOffersOfRacs(racs.RentACarServiceId);

                List<object> objs = new List<object>();

                foreach (var item in specOffers)
                {
                    var sum = 0.0;
                    foreach (var r in item.Car.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Car.Rates.ToArray().Length;
                    objs.Add(new
                    {
                        racs.Name,
                        item.NewPrice,
                        item.OldPrice,
                        FromDate = item.FromDate.Date,
                        ToDate = item.ToDate.Date,
                        item.Car.Brand,
                        item.Car.CarId,
                        item.Car.ImageUrl,
                        item.Car.Model,
                        item.Car.SeatsNumber,
                        item.Car.Type,
                        item.Car.Year,
                        rate = rate
                    });
                }

                return Ok(objs);

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return special offers");
            }
        }

        #endregion

        #region Change info methods
        [HttpPut]
        [Route("change-racs-info")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeRACSInfo(int id, ChangeRACSInfoDto dto)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new IdentityError() { Code = "", Description = "id empty" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId, null, "Address");
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return BadRequest("Racs doesnt exist");
                }

                racs.Name = dto.Name;
                racs.About = dto.PromoDescription;

                bool addressChanged = false;

                if (!racs.Address.City.Equals(dto.Address.City) || !racs.Address.City.Equals(dto.Address.State) ||
                    !racs.Address.Lat.Equals(dto.Address.Lat) || !racs.Address.Lon.Equals(dto.Address.Lon))
                {
                    racs.Address.City = dto.Address.City;
                    racs.Address.State = dto.Address.State;
                    racs.Address.Lon = dto.Address.Lon;
                    racs.Address.Lat = dto.Address.Lat;
                    addressChanged = true;
                }

                if (addressChanged)
                {
                    //using (var transaction = new TransactionScope())
                    //{
                    try
                    {
                        unitOfWork.RentCarServiceRepository.Update(racs);
                        //unitOfWork.Commit();
                        await unitOfWork.RentCarServiceRepository.UpdateAddress(racs.Address);
                        await unitOfWork.Commit();

                        //transaction.Complete();
                        //await transaction.Result.CommitAsync();
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Failed to apply changes");
                        //unitOfWork.Rollback();
                        //transaction.Dispose();
                        //await transaction.Result.RollbackAsync();
                    }
                    //}
                }
                else
                {
                    try
                    {
                        unitOfWork.RentCarServiceRepository.Update(racs);
                        await unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Failed to apply changes");
                    }
                }
                //racs.Address = dto.Address;
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to apply changes");
            }
        }

        [HttpPut]
        [Route("change-racs-logo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeRACSLogo(IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                string userRole = User.Claims.First(c => c.Type == "Roles").Value;

                if (!userRole.Equals("RentACarServiceAdmin"))
                {
                    return Unauthorized();
                }

                HttpStatusCode result = (await httpClient.GetAsync(String.Format("http://usermicroservice:80/api/user/find-user?id={0}", userId))).StatusCode;

                if (result.Equals(HttpStatusCode.NotFound))
                {
                    return NotFound();
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId);
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return BadRequest("Racs doesnt exist");
                }

                using (var stream = new MemoryStream())
                {
                    await img.CopyToAsync(stream);
                    racs.LogoUrl = stream.ToArray();
                }
                try
                {
                    unitOfWork.RentCarServiceRepository.Update(racs);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to apply changes");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to apply changes");
            }
        }
        #endregion
    }
}
