using Microsoft.AspNetCore.Mvc;
using RACSMicroservice.Models;
using RACSMicroservice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RACSMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;

        public HomeController(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            this.unitOfWork = unitOfWork;
            this.httpClient = httpClient;
        }

        [HttpGet]
        [Route("get-toprated-racs")]
        public async Task<IActionResult> GetTopRatedRACS()
        {
            var racs = await unitOfWork.RentCarServiceRepository.Get(null, null, "Rates,Address,Branches");

            var tupleList = new List<Tuple<float, object>>();

            float sum = 0;

            foreach (var item in racs)
            {
                var branches = new List<object>();
                foreach (var d in item.Branches)
                {
                    branches.Add(new
                    {
                        City = d.City,
                        State = d.State
                    });
                }

                sum = 0;

                foreach (var r in item.Rates)
                {
                    sum += r.Rate;
                }

                float rate = sum == 0 ? 0 : (float)sum / item.Rates.ToArray().Length;

                var obj = new
                {
                    Name = item.Name,
                    Logo = item.LogoUrl,
                    City = item.Address.City,
                    State = item.Address.State,
                    rate = rate,
                    About = item.About,
                    Id = item.RentACarServiceId,
                    Branches = branches,
                };

                tupleList.Add(new Tuple<float, object>(obj.rate, obj));
            }

            var ordredList = tupleList.OrderByDescending(x => x.Item1).Take(5);

            var retlist = new List<object>();

            foreach (var item in ordredList)
            {
                retlist.Add(item.Item2);
            }

            return Ok(retlist);
        }
        [HttpGet]
        [Route("rent-car-services")]
        public async Task<IActionResult> RentCarServices()
        {
            try
            {
                var queryString = Request.Query;
                var sortType = queryString["typename"].ToString();
                var sortByName = queryString["sortbyname"].ToString();
                var sortTypeCity = queryString["typecity"].ToString();
                var sortByCity = queryString["sortbycity"].ToString();

                var services = await unitOfWork.RentCarServiceRepository.Get(null, null, "Branches,Address,Rates");

                if (!String.IsNullOrEmpty(sortByCity) && !String.IsNullOrEmpty(sortTypeCity))
                {
                    if (sortType.Equals("ascending"))
                    {
                        services.OrderBy(a => a.Address.City);

                    }
                    else
                    {
                        services.OrderByDescending(a => a.Address.City);
                    }
                }
                if (!String.IsNullOrEmpty(sortByName) && !String.IsNullOrEmpty(sortType))
                {
                    if (sortType.Equals("ascending"))
                    {
                        services.OrderBy(a => a.Name);
                    }
                    else
                    {
                        services.OrderByDescending(a => a.Name);
                    }
                }

                List<object> retList = new List<object>();
                List<object> branches = new List<object>();

                foreach (var item in services)
                {
                    branches = new List<object>();
                    foreach (var d in item.Branches)
                    {
                        branches.Add(new
                        {
                            City = d.City,
                            State = d.State
                        });
                    }

                    var sum = 0.0;
                    foreach (var r in item.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Rates.ToArray().Length;

                    //var rate = 0.0;
                    //if (item.Rates.Count > 0)
                    //{
                    //    foreach (var r in item.Rates)
                    //    {
                    //        rate += r.Rate;
                    //    }

                    //    rate = rate / item.Rates.Count();
                    //}

                    retList.Add(new
                    {
                        Name = item.Name,
                        Logo = item.LogoUrl,
                        City = item.Address.City,
                        State = item.Address.State,
                        //Lon = item.Address.Lon,
                        //Lat = item.Address.Lat,
                        rate = rate,
                        About = item.About,
                        Id = item.RentACarServiceId,
                        Branches = branches,
                    });
                }

                return Ok(retList);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return RACS");
            }
        }

        [HttpGet]
        [Route("rent-car-service/{id}")]
        public async Task<IActionResult> GetRentCarService(int id)
        {
            try
            {
                var res = await unitOfWork.RentCarServiceRepository.Get(racs => racs.RentACarServiceId == id, null, "Branches,Address,Rates");
                var rentservice = res.FirstOrDefault();

                List<object> branches = new List<object>();
                foreach (var item in rentservice.Branches)
                {
                    branches.Add(new
                    {
                        item.City,
                        BranchId = item.BranchId
                    });
                }

                var sum = 0.0;
                foreach (var r in rentservice.Rates)
                {
                    sum += r.Rate;
                }

                float rate = sum == 0 ? 0 : (float)sum / rentservice.Rates.ToArray().Length;

                object obj = new
                {
                    Name = rentservice.Name,
                    About = rentservice.About,
                    City = rentservice.Address.City,
                    State = rentservice.Address.State,
                    Lat = rentservice.Address.Lat,
                    Lon = rentservice.Address.Lon,
                    Logo = rentservice.LogoUrl,
                    Id = rentservice.RentACarServiceId,
                    Branches = branches,
                    rate = rate
                };

                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return RACS");
            }
        }

        [HttpGet]
        [Route("cars")]
        public async Task<IActionResult> GetAllCars()
        {
            try
            {
                var queryString = Request.Query;
                var fromDate = queryString["dep"].ToString();
                var toDate = queryString["ret"].ToString();
                DateTime DateFrom = DateTime.Now;
                DateTime DateTo = DateTime.Now;

                if (!String.IsNullOrEmpty(fromDate))
                {
                    DateFrom = Convert.ToDateTime(fromDate);
                }
                if (DateFrom < DateTime.Now.Date)
                {
                    return BadRequest("Selected date is in past. Choose another date.");
                }
                if (!String.IsNullOrEmpty(toDate))
                {
                    DateTo = Convert.ToDateTime(toDate);
                }
                if (DateFrom > DateTo)
                {
                    return BadRequest("From date should be lower then to date");
                }

                var fromCity = queryString["from"].ToString();
                var toCity = queryString["to"].ToString();
                var carType = queryString["type"].ToString();

                int seatFrom;
                int seatTo;
                Int32.TryParse(queryString["seatfrom"].ToString(), out seatFrom);
                Int32.TryParse(queryString["seatto"].ToString(), out seatTo);
                seatFrom = seatFrom == 0 ? 2 : seatFrom;
                seatTo = seatTo == 0 ? 10 : seatTo;

                float priceFrom = 0;
                float priceTo = 3000;
                float.TryParse(queryString["minprice"].ToString(), out priceFrom);
                float.TryParse(queryString["maxprice"].ToString(), out priceTo);

                int num = 0;

                List<int> ids = new List<int>();
                if (!String.IsNullOrEmpty(queryString["racs"].ToString()))
                {
                    foreach (var item in queryString["racs"].ToString().Trim().Split(','))
                    {
                        if (!Int32.TryParse(item, out num))
                        {
                            return BadRequest("Wrong parameter");
                        }
                        ids.Add(num);
                    }
                }

                int numOfDays = (int)(DateTo - DateFrom).TotalDays;


                var allCars = await unitOfWork.CarRepository.AllCars(c =>
                                    (c.RentACarService != null ? c.RentACarService.Address.City == fromCity
                                    && (c.RentACarService.Branches.FirstOrDefault(b => b.City == toCity) != null || c.RentACarService.Address.City == toCity)
                                    : c.Branch.City == fromCity && (c.Branch.RentACarService.Branches.FirstOrDefault(b => b.City == toCity) != null || c.Branch.RentACarService.Address.City == toCity))
                                    && c.PricePerDay * numOfDays >= priceFrom && c.PricePerDay * numOfDays <= priceTo
                                    && c.SeatsNumber >= seatFrom && c.SeatsNumber <= seatTo);

                List<object> objs = new List<object>();
                RentACarService rentService = new RentACarService();
                Branch branch = new Branch();
                Address address = new Address();

                string name;
                byte[] logo;

                foreach (var item in allCars)
                {

                    if (item.Branch == null)
                    {
                        rentService = item.RentACarService;
                        address = rentService.Address;
                        logo = rentService.LogoUrl;
                        name = rentService.Name;
                    }
                    else
                    {
                        branch = item.Branch;
                        address.City = branch.City;
                        address.State = branch.State;
                        logo = branch.RentACarService.LogoUrl;
                        name = branch.RentACarService.Name;
                    }

                    if (!FilterPass(item, ids, priceFrom, priceTo,
                        fromCity, toCity, carType, DateFrom.Date, DateTo.Date,
                        seatFrom, seatTo))
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

                        item.CarId,
                        item.ImageUrl,
                        item.Model,
                        item.PricePerDay,
                        item.SeatsNumber,
                        item.Type,
                        item.Year,
                        item.Brand,
                        address.City,
                        address.State,
                        logo,
                        name,
                        rate = rate,
                    });
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return cars");
            }
        }

        [HttpGet]
        [Route("car/{id}")]
        public async Task<IActionResult> Car(int id)
        {
            try
            {
                var res = await unitOfWork.CarRepository.Get(c => c.CarId == id, null, "Branch,RentACarService,Rates");
                var car = res.FirstOrDefault();

                var sum = 0.0;
                foreach (var r in car.Rates)
                {
                    sum += r.Rate;
                }

                float rate = sum == 0 ? 0 : (float)sum / car.Rates.ToArray().Length;

                var obj = new
                {
                    car.CarId,
                    car.ImageUrl,
                    car.Model,
                    car.PricePerDay,
                    car.SeatsNumber,
                    car.Type,
                    car.Year,
                    car.Brand,
                    Name = car.Branch == null ? car.RentACarService.Name : car.Branch.RentACarService.Name,
                    Logo = car.Branch == null ? car.RentACarService.LogoUrl : car.Branch.RentACarService.LogoUrl,
                    rate = rate
                };

                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return car");
            }
        }

        [HttpGet]
        [Route("racs-specialoffers/{id}")]
        public async Task<IActionResult> RacsSpecialOffers(int id)
        {
            try
            {
                var specOffers = await unitOfWork.RACSSpecialOfferRepository.GetSpecialOffersOfRacs(id);

                List<object> objs = new List<object>();

                foreach (var item in specOffers)
                {
                    if (item.FromDate < DateTime.Now.Date)
                    {
                        continue;
                    }

                    var sum = 0.0;
                    foreach (var r in item.Car.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Car.Rates.ToArray().Length;

                    objs.Add(new
                    {
                        Name = item.Car.Branch == null ? item.Car.RentACarService.Name : item.Car.Branch.RentACarService.Name,
                        Logo = item.Car.Branch == null ? item.Car.RentACarService.LogoUrl : item.Car.Branch.RentACarService.LogoUrl,
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
                        Id = item.CarSpecialOfferId,
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
        [HttpGet]
        [Route("all-racs-specialoffers")]
        public async Task<IActionResult> AllRacsSpecialOffers(int id)
        {
            try
            {
                var specOffers = await unitOfWork.RACSSpecialOfferRepository.GetSpecialOffersOfAllRacs();

                List<object> objs = new List<object>();

                foreach (var item in specOffers)
                {
                    if (item.FromDate < DateTime.Now.Date || item.IsReserved)
                    {
                        continue;
                    }

                    var sum = 0.0;
                    foreach (var r in item.Car.Rates)
                    {
                        sum += r.Rate;
                    }

                    float rate = sum == 0 ? 0 : (float)sum / item.Car.Rates.ToArray().Length;

                    objs.Add(new
                    {
                        Name = item.Car.Branch == null ? item.Car.RentACarService.Name : item.Car.Branch.RentACarService.Name,
                        Logo = item.Car.Branch == null ? item.Car.RentACarService.LogoUrl : item.Car.Branch.RentACarService.LogoUrl,
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
                        Id = item.CarSpecialOfferId,
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
        private bool FilterPass(Car item, List<int> ids, float priceFrom, float priceTo, string fromCity,
            string toCity, string carType, DateTime from, DateTime to, int seatFrom, int seatTo)
        {
            //int numOfDays = Math.Abs(from.Day - to.Day);
            //int numOfDays = (int)(to - from).TotalDays;
            if (ids.Count > 0)
            {
                if (item.RentACarService != null)
                {
                    if (!ids.Contains(item.RentACarService.RentACarServiceId))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!ids.Contains(item.Branch.RentACarServiceId))
                    {
                        return false;
                    }
                }
            }
            //if (item.PricePerDay * numOfDays < priceFrom || item.PricePerDay * numOfDays > priceTo)
            //{
            //    return false;
            //}

            if (!item.Type.ToLower().Equals(carType.ToLower()) && carType != "")
            {
                return false;
            }
            //RentACarService racs = null;
            //if (item.RentACarService != null)
            //{
            //    racs = item.RentACarService;
            //}
            //else
            //{
            //    racs = item.Branch.RentACarService;
            //}

            foreach (var res in item.Rents)
            {
                if (from >= res.TakeOverDate && from <= res.ReturnDate
                    || to >= res.TakeOverDate && to <= res.ReturnDate
                    || from <= res.TakeOverDate && to >= res.ReturnDate)
                {
                    return false;
                }
            }

            foreach (var offer in item.SpecialOffers)
            {
                if (from >= offer.FromDate && from <= offer.ToDate
                    || to >= offer.FromDate && to <= offer.ToDate
                    || from <= offer.FromDate && to >= offer.ToDate)
                {
                    return false;
                }
                //if (offer.FromDate.Date >= from && offer.FromDate <= to || offer.ToDate.Date >= from && offer.ToDate <= to)
                //{
                //    return false;
                //}
            }

            //if (item.SeatsNumber < seatFrom || item.SeatsNumber > seatTo)
            //{
            //    return false;
            //}
            //bool fromFound = false;
            //bool toFound = false;

            //if (racs.Address.City == fromCity)
            //{
            //    fromFound = true;
            //}
            //if (racs.Address.City == toCity)
            //{
            //    toFound = true;
            //}

            //foreach (var branch in racs.Branches)
            //{
            //    if (branch.City == fromCity && !fromFound)
            //    {
            //        fromFound = true;
            //    }
            //    if (branch.City == toCity && !toFound)
            //    {
            //        toFound = true;
            //    }
            //}

            //if (!toFound || !fromFound)
            //{
            //    return false;
            //}


            return true;
        }
    }
}
