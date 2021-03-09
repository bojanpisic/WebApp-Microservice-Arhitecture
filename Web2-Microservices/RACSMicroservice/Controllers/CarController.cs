using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public CarController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
        }
        #region Car methods

        [HttpGet]
        [Route("get-car-report")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReport()  //  dobijaju izveštaje o slobodnim i zauzetim vozilima za određeni period
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


                var racss = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId, null, "Address");
                var racs = racss.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                var queryString = Request.Query;
                var from = queryString["from"].ToString();
                var to = queryString["to"].ToString();
                var isFree = queryString["isFree"].ToString().ToUpper();

                var fromDate = DateTime.Now;
                var toDate = DateTime.Now;

                if (!DateTime.TryParse(from, out fromDate))
                {
                    return BadRequest("Incorrect date format");
                }

                if (!DateTime.TryParse(to, out toDate))
                {
                    return BadRequest("Incorrect date format");
                }

                var allCars =
                    await unitOfWork.CarRepository
                    .Get(car => car.Branch.RentACarService.RentACarServiceId == racs.RentACarServiceId
                    || car.RentACarService.RentACarServiceId == racs.RentACarServiceId,
                    null, "Rents,Rates");

                List<object> objs = new List<object>();

                foreach (var item in allCars)
                {
                    //PROVERA DA LI POSTOJI REZERVACIJA U TOM PERIODU
                    var rent = item.Rents.FirstOrDefault(rent =>
                    rent.TakeOverDate >= fromDate && rent.ReturnDate <= toDate ||
                    rent.TakeOverDate <= fromDate && rent.ReturnDate >= toDate ||
                    rent.TakeOverDate <= fromDate && rent.ReturnDate <= toDate && rent.ReturnDate >= fromDate ||
                    rent.TakeOverDate >= fromDate && rent.TakeOverDate <= toDate && rent.ReturnDate >= toDate);

                    if (isFree.Equals("TRUE") && rent != null || isFree.Equals("FALSE") && rent == null)
                    {
                        continue;
                    }

                    var sum = 0.0;
                    foreach (var r in item.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Rates.ToArray().Length;

                    objs.Add(new
                    {
                        racs.Name,
                        item.CarId,
                        item.ImageUrl,
                        item.Model,
                        item.PricePerDay,
                        item.SeatsNumber,
                        item.Type,
                        item.Year,
                        item.Brand,
                        racs.Address.City,
                        racs.Address.State,
                        isFree = rent == null ? true : false,
                        rate = rate
                    });
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return reports");
            }
        }


        [HttpGet]
        [Route("get-racs-cars")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRACSCars()
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

                var racs = await unitOfWork.RentCarServiceRepository.GetRACSAndCars(userId); //kupi i od filijala auta

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                var allCars = racs.Cars;

                List<object> objs = new List<object>();

                foreach (var item in allCars)
                {

                    var sum = 0.0;
                    foreach (var r in item.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Rates.ToArray().Length;

                    objs.Add(new
                    {
                        racs.Name,
                        item.CarId,
                        item.ImageUrl,
                        item.Model,
                        item.PricePerDay,
                        item.SeatsNumber,
                        item.Type,
                        item.Year,
                        item.Brand,
                        racs.Address.City,
                        racs.Address.State,
                        rate = rate
                    });
                }

                foreach (var branch in racs.Branches)
                {
                    foreach (var car in branch.Cars)
                    {
                        var sum = 0.0;
                        foreach (var r in car.Rates)
                        {
                            sum += r.Rate;
                        }

                        float rate = sum == 0 ? 0 : (float)sum / car.Rates.ToArray().Length;

                        objs.Add(new
                        {
                            racs.Name,
                            car.CarId,
                            car.ImageUrl,
                            car.Model,
                            car.PricePerDay,
                            car.SeatsNumber,
                            car.Type,
                            car.Year,
                            car.Brand,
                            branch.City,
                            branch.State,
                            rate = rate
                        });
                    }
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return cars");
            }
        }
        [HttpGet]
        [Route("get-branch-cars/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetBranchCars(int id)
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

                //var res = await unitOfWork.Branchrepository.Get(b => b.BranchId == id, null, "Cars,RentACarService");
                var res = await unitOfWork.CarRepository.CarsOfBranch(id);

                //var branch = res.FirstOrDefault();

                //if (branch == null)
                //{
                //    return BadRequest("Branch not found");
                //}

                //var allCars = branch.Cars;

                List<object> objs = new List<object>();

                foreach (var item in res) //bilo allcars
                {
                    var sum = 0.0;
                    foreach (var r in item.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Rates.ToArray().Length;

                    objs.Add(new
                    {
                        item.Branch.RentACarService.Name,
                        item.CarId,
                        item.ImageUrl,
                        item.Model,
                        item.PricePerDay,
                        item.SeatsNumber,
                        item.Type,
                        item.Year,
                        item.Brand,
                        rate = rate
                    });
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return branch cars");
            }
        }
        [HttpPost]
        [Route("add-car")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddCarToService([FromBody] CarDto carDto)
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

                if (carDto.PricePerDay < 0)
                {
                    return BadRequest("Price should be greater then 0");
                }

                if (carDto.SeatsNumber < 2 || carDto.SeatsNumber > 10)
                {
                    return BadRequest("Seats number can be between 2 and 10");
                }

                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId);
                var racs = res.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("Racs not found");
                }
                var car = new Car()
                {
                    RentACarService = racs,
                    Branch = null,
                    Model = carDto.Model,
                    PricePerDay = carDto.PricePerDay,
                    Year = carDto.Year,
                    SeatsNumber = carDto.SeatsNumber,
                    Type = carDto.Type,
                    Brand = carDto.Brand
                };
                //using (var stream = new MemoryStream())
                //{
                //    await img.CopyToAsync(stream);
                //    car.ImageUrl = stream.ToArray();
                //}


                try
                {
                    await unitOfWork.CarRepository.Insert(car);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to add car");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add car");
            }
        }

        [HttpPost]
        [Route("add-car-to-branch")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> AddCarToBranch([FromBody] CarDto carDto)
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

                if (carDto.PricePerDay < 0)
                {
                    return BadRequest("Price should be greater then 0");
                }

                var branch = await unitOfWork.BranchRepository.GetByID(carDto.BranchId);

                if (branch == null)
                {
                    return NotFound("Racs not found");
                }
                var car = new Car()
                {
                    RentACarService = null,
                    Branch = branch,
                    Model = carDto.Model,
                    PricePerDay = carDto.PricePerDay,
                    Year = carDto.Year,
                    SeatsNumber = carDto.SeatsNumber,
                    Type = carDto.Type,
                    Brand = carDto.Brand
                };

                try
                {
                    await unitOfWork.CarRepository.Insert(car);
                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to add car to branch");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add car to branch");
            }
        }

        [HttpPut]
        [Route("change-car-info/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeCarInfo(int id, ChangeCarDto dto)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("id empty");
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

                if (dto.PricePerDay < 0)
                {
                    return BadRequest("Price should be greater then 0");
                }

                var cars = await unitOfWork.CarRepository.Get(car => car.CarId == id, null, "Rents");
                var car = cars.FirstOrDefault();

                if (car == null)
                {
                    return NotFound("Car not found");
                }

                if (car.Rents.FirstOrDefault(rent => rent.TakeOverDate <= DateTime.Now && rent.TakeOverDate >= DateTime.Now) != null)
                {
                    return BadRequest("Cant modifie this car");
                }

                car.Brand = dto.Brand;
                car.Model = dto.Model;
                car.PricePerDay = dto.PricePerDay;
                car.SeatsNumber = dto.SeatsNumber;
                car.Type = dto.Type;
                car.Year = dto.Year;

                try
                {
                    unitOfWork.CarRepository.Update(car);
                    await unitOfWork.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest("Something is changed. Cant change");
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

        [HttpPut]
        [Route("change-car-image/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> ChangeCarImage(IFormFile img, int id)
        {
            if (img == null)
            {
                return BadRequest();
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

                var cars = await unitOfWork.CarRepository.Get(car => car.CarId == id, null, "Rents");
                var car = cars.FirstOrDefault();

                if (car == null)
                {
                    return NotFound("Car not found");
                }

                if (car.Rents.FirstOrDefault(rent => rent.TakeOverDate <= DateTime.Now && rent.TakeOverDate >= DateTime.Now) != null)
                {
                    return BadRequest("Cant modifie this car");
                }

                using (var stream = new MemoryStream())
                {
                    await img.CopyToAsync(stream);
                    car.ImageUrl = stream.ToArray();
                }

                try
                {
                    unitOfWork.CarRepository.Update(car);
                    await unitOfWork.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest("Something is changed. Cant change");
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


        [HttpGet]
        [Route("get-car/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCar(int id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest();
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

                var res = await unitOfWork.CarRepository.Get(c => c.CarId == id, null, "SpecialOffers,Rates");
                var car = res.FirstOrDefault();

                if (car == null)
                {
                    return NotFound("Car doesnt exist");
                }

                var ress = await unitOfWork.RentCarServiceRepository.Get(racs => racs.AdminId == userId);
                var racs = ress.FirstOrDefault();

                if (racs == null)
                {
                    return NotFound("RACS not found");
                }

                List<object> specOffDates = new List<object>();

                foreach (var item in car.SpecialOffers)
                {
                    specOffDates.Add(new { From = item.FromDate, To = item.ToDate });
                }

                var sum = 0.0;
                foreach (var r in car.Rates)
                {
                    sum += r.Rate;
                }

                float rate = sum == 0 ? 0 : (float)sum / car.Rates.ToArray().Length;

                var obj = new
                {
                    racs.Name,
                    car.CarId,
                    car.ImageUrl,
                    car.Model,
                    car.PricePerDay,
                    car.SeatsNumber,
                    car.Type,
                    car.Year,
                    car.Brand,
                    SpecialOfferDatesOfCar = specOffDates,
                    rate = rate
                };

                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return car");
            }
        }

        [HttpDelete]
        [Route("delete-car/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest();
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

                var cars = await unitOfWork.CarRepository.Get(car => car.CarId == id, null, "Rents");
                var car = cars.FirstOrDefault();

                if (car == null)
                {
                    return NotFound("Car not found");
                }

                if (car.Rents.FirstOrDefault(rent => rent.TakeOverDate <= DateTime.Now && rent.TakeOverDate >= DateTime.Now) != null)
                {
                    return BadRequest("Cant delete this car");
                }

                try
                {
                    unitOfWork.CarRepository.Delete(car);
                    await unitOfWork.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest("Something is changed. Cant delete");
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to delete car");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete car");
            }
        }

        #endregion

        [HttpPost]
        [Route("rate-car")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RateCar(RateDto dto)
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

                var rent = await unitOfWork.CarRentRepository.GetRentByFilter(r => r.UserId == userId && r.RentedCar.CarId == dto.Id);

                //var rents = await unitOfWork.CarRentRepository.Get(crr => crr.User == user, null, "RentedCar");

                //var rent = rents.FirstOrDefault(r => r.RentedCar.CarId == dto.Id);

                if (rent == null)
                {
                    return BadRequest("This car is not on your rent list");
                }
                if (rent.RentedCar.Rates.FirstOrDefault(r => r.UserId == userId) != null)
                {
                    return BadRequest("You already rated this car");
                }

                if (rent.ReturnDate > DateTime.Now)
                {
                    return BadRequest("You can rate this car only when rate period expires");
                }

                var rentedCar = rent.RentedCar;

                rentedCar.Rates.Add(new CarRate()
                {
                    Rate = dto.Rate,
                    UserId = userId,
                    Car = rentedCar
                });

                try
                {
                    unitOfWork.CarRepository.Update(rentedCar);

                    await unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Failed to rate car. One of transactions failed");
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rate car");
            }
        }
    }
}
