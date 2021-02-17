using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACSMicroservice.Models;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RACSMicroservice.Controllers
{
    public class ChartController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public ChartController(IUnitOfWork _unitOfWork, HttpClient httpClient)
        {
            unitOfWork = _unitOfWork;
            this.httpClient = httpClient;
        }
        
        #region Chart methods
        [HttpGet]
        [Route("get-stats-date")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDayStats()
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

                var queryString = Request.Query;
                var date = queryString["date"].ToString();

                var day = DateTime.Now;

                if (!DateTime.TryParse(date, out day))
                {
                    return BadRequest("Date format is incorrect");
                }

                var cars = await unitOfWork.CarRepository.Get(car =>
                    car.RentACarService == null ?
                    car.Branch.RentACarService.AdminId == userId : car.RentACarService.AdminId == userId,
                    null, "Rents");

                int rentNum = 0;

                foreach (var item in cars)
                {
                    if (item.Rents.FirstOrDefault(rent => rent.RentDate == day) != null)
                    {
                        rentNum++;
                    }
                }

                return Ok(new Tuple<DateTime, int>(day, rentNum));
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get day state");
            }
        }

        [HttpGet]
        [Route("get-stats-week")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetWeekStats()
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

                var queryString = Request.Query;
                var week = queryString["week"].ToString().Split("-W")[1];
                var year = queryString["week"].ToString().Split("-W")[0];

                int weekNum = 0;
                int yearNum = 0;

                if (!Int32.TryParse(week, out weekNum))
                {
                    return BadRequest();
                }
                if (!Int32.TryParse(year, out yearNum))
                {
                    return BadRequest();
                }

                List<DateTime> daysOfWeek = new List<DateTime>();

                var lastDay = new DateTime(yearNum, 1, 1).AddDays((weekNum) * 7);
                daysOfWeek.Add(lastDay);

                for (int i = 1; i < 7; i++)
                {
                    daysOfWeek.Add(lastDay.AddDays(-i));
                }

                var cars = await unitOfWork.CarRepository.Get(car =>
                    car.RentACarService == null ?
                    car.Branch.RentACarService.AdminId == userId : car.RentACarService.AdminId == userId,
                    null, "Rents");

                List<Tuple<DateTime, int>> stats = new List<Tuple<DateTime, int>>();

                foreach (var day in daysOfWeek)
                {
                    stats.Add(new Tuple<DateTime, int>(day, 0));
                }

                CarRent r;

                foreach (var item in cars)
                {
                    if ((r = item.Rents.FirstOrDefault(rent => daysOfWeek.Contains(rent.RentDate))) != null)
                    {
                        var s = stats.Find(s => s.Item1 == r.TakeOverDate);
                        int index = stats.IndexOf(s);

                        stats[index] = new Tuple<DateTime, int>(s.Item1, s.Item2 + 1);
                    }
                }

                return Ok(stats);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get day state");
            }
        }

        [HttpGet]
        [Route("get-stats-month")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMonthStats()
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

                var queryString = Request.Query;
                var month = queryString["month"].ToString().Split("-")[1];
                var year = queryString["month"].ToString().Split("-")[0];

                int monthNum = 0;
                int yearNum = 0;

                if (!Int32.TryParse(month, out monthNum))
                {
                    return BadRequest();
                }
                if (!Int32.TryParse(year, out yearNum))
                {
                    return BadRequest();
                }

                int numOfDays = DateTime.DaysInMonth(yearNum, monthNum);
                DateTime firstDayOfMonth = new DateTime(yearNum, monthNum, 1);


                List<DateTime> daysOfMonth = new List<DateTime>();

                daysOfMonth.Add(firstDayOfMonth);

                for (int i = 1; i < numOfDays; i++)
                {
                    daysOfMonth.Add(firstDayOfMonth.AddDays(i));
                }

                var cars = await unitOfWork.CarRepository.Get(car =>
                    car.RentACarService == null ?
                    car.Branch.RentACarService.AdminId == userId : car.RentACarService.AdminId == userId,
                    null, "Rents");

                List<Tuple<DateTime, int>> stats = new List<Tuple<DateTime, int>>();

                foreach (var day in daysOfMonth)
                {
                    stats.Add(new Tuple<DateTime, int>(day, 0));
                }

                CarRent r;

                foreach (var item in cars)
                {
                    if ((r = item.Rents.FirstOrDefault(rent => daysOfMonth.Contains(rent.RentDate))) != null)
                    {
                        var s = stats.Find(s => s.Item1 == r.TakeOverDate);
                        int index = stats.IndexOf(s);

                        stats[index] = new Tuple<DateTime, int>(s.Item1, s.Item2 + 1);
                    }
                }

                return Ok(stats);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get day state");
            }
        }


        [HttpGet]
        [Route("get-income-week")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetWeekIncome()
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

                var queryString = Request.Query;
                var week = queryString["week"].ToString().Split("-W")[1];
                var year = queryString["week"].ToString().Split("-W")[0];

                int weekNum = 0;
                int yearNum = 0;

                if (!Int32.TryParse(week, out weekNum))
                {
                    return BadRequest();
                }
                if (!Int32.TryParse(year, out yearNum))
                {
                    return BadRequest();
                }

                List<DateTime> daysOfWeek = new List<DateTime>();

                var lastDay = new DateTime(yearNum, 1, 1).AddDays((weekNum) * 7);
                daysOfWeek.Add(lastDay);

                for (int i = 1; i < 7; i++)
                {
                    daysOfWeek.Add(lastDay.AddDays(-i));
                }

                var cars = await unitOfWork.CarRepository.Get(car =>
                    car.RentACarService == null ?
                    car.Branch.RentACarService.AdminId == userId : car.RentACarService.AdminId == userId,
                    null, "Rents");

                List<Tuple<DateTime, float>> income = new List<Tuple<DateTime, float>>();

                foreach (var day in daysOfWeek)
                {
                    income.Add(new Tuple<DateTime, float>(day, 0));
                }

                CarRent r;

                foreach (var item in cars)
                {
                    if ((r = item.Rents.FirstOrDefault(rent => daysOfWeek.Contains(rent.RentDate))) != null)
                    {
                        var s = income.Find(s => s.Item1 == r.RentDate);
                        int index = income.IndexOf(s);

                        income[index] = new Tuple<DateTime, float>(s.Item1, s.Item2 + r.TotalPrice);
                    }
                }

                return Ok(income);

            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get day state");
            }
        }

        [HttpGet]
        [Route("get-income-month")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMonthIncome()
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

                var queryString = Request.Query;
                var month = queryString["month"].ToString().Split("-")[1];
                var year = queryString["month"].ToString().Split("-")[0];

                int monthNum = 0;
                int yearNum = 0;

                if (!Int32.TryParse(month, out monthNum))
                {
                    return BadRequest();
                }
                if (!Int32.TryParse(year, out yearNum))
                {
                    return BadRequest();
                }

                int numOfDays = DateTime.DaysInMonth(yearNum, monthNum);
                DateTime firstDayOfMonth = new DateTime(yearNum, monthNum, 1);


                List<DateTime> daysOfMonth = new List<DateTime>();

                daysOfMonth.Add(firstDayOfMonth);

                for (int i = 1; i < numOfDays; i++)
                {
                    daysOfMonth.Add(firstDayOfMonth.AddDays(i));
                }

                var cars = await unitOfWork.CarRepository.Get(car =>
                    car.RentACarService == null ?
                    car.Branch.RentACarService.AdminId == userId : car.RentACarService.AdminId == userId,
                    null, "Rents");

                List<Tuple<DateTime, float>> income = new List<Tuple<DateTime, float>>();

                foreach (var day in daysOfMonth)
                {
                    income.Add(new Tuple<DateTime, float>(day, 0));
                }

                CarRent r;

                foreach (var item in cars)
                {
                    if ((r = item.Rents.FirstOrDefault(rent => daysOfMonth.Contains(rent.RentDate))) != null)
                    {
                        var s = income.Find(s => s.Item1 == r.TakeOverDate);
                        int index = income.IndexOf(s);

                        income[index] = new Tuple<DateTime, float>(s.Item1, s.Item2 + r.TotalPrice);
                    }
                }

                return Ok(income);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get day state");
            }
        }

        [HttpGet]
        [Route("get-income-year")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetYearIncome()
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

                var queryString = Request.Query;
                var year = queryString["year"].ToString();

                int yearNum = 0;

                if (!Int32.TryParse(year, out yearNum))
                {
                    return BadRequest();
                }
                if (yearNum < 1)
                {
                    return BadRequest();
                }

                var retVal = new List<Tuple<string, float>>();

                var cars = await unitOfWork.CarRepository.Get(car =>
                                                            car.RentACarService == null ?
                                                            car.Branch.RentACarService.AdminId == userId : car.RentACarService.AdminId == userId,
                                                            null, "Rents");

                for (int m = 1; m < 13; m++)
                {

                    int numOfDays = DateTime.DaysInMonth(yearNum, m);
                    DateTime firstDayOfMonth = new DateTime(yearNum, m, 1);


                    List<DateTime> daysOfMonth = new List<DateTime>();

                    daysOfMonth.Add(firstDayOfMonth);

                    for (int i = 1; i < numOfDays; i++)
                    {
                        daysOfMonth.Add(firstDayOfMonth.AddDays(i));
                    }

                    float monthIncome = 0;

                    CarRent r;

                    foreach (var item in cars)
                    {
                        if ((r = item.Rents.FirstOrDefault(rent => daysOfMonth.Contains(rent.RentDate))) != null)
                        {
                            monthIncome += r.TotalPrice;
                        }
                    }

                    string monthName = new DateTime(yearNum, m, 1)
                         .ToString("MMM", CultureInfo.InvariantCulture);

                    retVal.Add(new Tuple<string, float>(monthName, monthIncome));
                }

                return Ok(retVal);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to get day state");
            }
        }

        #endregion
    }
}
