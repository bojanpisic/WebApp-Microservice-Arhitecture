using AirlineMicroservice.Models;
using AirlineMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        public HomeController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test() 
        {
            return Ok("daaaaaaaaaaa");
        }

        [HttpGet]
        [Route("get-toprated-airlines")]
        public async Task<IActionResult> GetTopRated()
        {
            try
            {
                var airlines = await unitOfWork.AirlineRepository.GetAllAirlines();

                var tupleList = new List<Tuple<float, object>>();

                float sum = 0;

                foreach (var item in airlines)
                {
                    sum = 0;
                    foreach (var rate in item.Rates)
                    {
                        sum += rate.Rate;
                    }

                    var allDest = new List<object>();

                    foreach (var dest in item.Destinations)
                    {
                        allDest.Add(new
                        {
                            dest.Destination.City,
                            dest.Destination.State
                        });
                    }

                    var airlineObj = new
                    {
                        AirlineId = item.AirlineId,
                        City = item.Address.City,
                        State = item.Address.State,
                        Lat = item.Address.Lat,
                        Lon = item.Address.Lon,
                        rate = sum == 0 ? 0 : sum / item.Rates.Count,
                        Name = item.Name,
                        Logo = item.LogoUrl,
                        About = item.PromoDescription,
                        Destinations = allDest
                    };
                    tupleList.Add(new Tuple<float, object>(airlineObj.rate, airlineObj));
                }

                var ordredList = tupleList.OrderByDescending(x => x.Item1).Take(5);

                var retlist = new List<object>();

                foreach (var item in ordredList)
                {
                    retlist.Add(item.Item2);
                }

                return Ok(retlist);
            }
            catch (Exception)
            {
                return StatusCode(500, "failed to return top rated airlines");
            }
            
        }

        [HttpGet]
        [Route("all-airlines")]
        public async Task<IActionResult> AllAirlinesForUser()
        {
            try
            {
                var queryString = Request.Query;
                var sortType = queryString["typename"].ToString();
                var sortByName = queryString["sortbyname"].ToString();
                var sortTypeCity = queryString["typecity"].ToString();
                var sortByCity = queryString["sortbycity"].ToString();

                var airlines = await unitOfWork.AirlineRepository.GetAllAirlines();


                if (!String.IsNullOrEmpty(sortByCity) && !String.IsNullOrEmpty(sortTypeCity))
                {
                    if (sortType.Equals("ascending"))
                    {
                        airlines.OrderBy(a => a.Address.City);

                    }
                    else
                    {
                        airlines.OrderByDescending(a => a.Address.City);
                    }
                }
                if (!String.IsNullOrEmpty(sortByName) && !String.IsNullOrEmpty(sortType))
                {
                    if (sortType.Equals("ascending"))
                    {
                        airlines.OrderBy(a => a.Name);
                    }
                    else
                    {
                        airlines.OrderByDescending(a => a.Name);
                    }
                }

                List<object> all = new List<object>();
                List<object> allDest = new List<object>();

                foreach (var item in airlines)
                {
                    allDest = new List<object>();

                    foreach (var dest in item.Destinations)
                    {
                        allDest.Add(new
                        {
                            dest.Destination.City,
                            dest.Destination.State
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

                    all.Add(new
                    {
                        AirlineId = item.AirlineId,
                        City = item.Address.City,
                        State = item.Address.State,
                        //Lat = item.Address.Lat,
                        //Lon = item.Address.Lon,
                        rate = rate,
                        Name = item.Name,
                        Logo = item.LogoUrl,
                        About = item.PromoDescription,
                        Destinations = allDest
                    });
                }

                return Ok(all);
            }
            catch (Exception)
            {

                return StatusCode(500, "Failed to return airlines");
            }
        }

        [HttpGet]
        [Route("airline/{id}")]
        public async Task<IActionResult> Airline(int id)
        {
            try
            {
                var airline = await unitOfWork.AirlineRepository.GetAirline(id);

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                List<object> allDest = new List<object>();


                allDest = new List<object>();

                foreach (var dest in airline.Destinations)
                {
                    allDest.Add(new
                    {
                        dest.Destination.City,
                        dest.Destination.State,
                        dest.Destination.ImageUrl
                    });
                }

                var sum = 0.0;
                foreach (var r in airline.Rates)
                {
                    sum += r.Rate;
                }

                float rate = sum == 0 ? 0 : (float)sum / airline.Rates.ToArray().Length;

                object obj = new
                {
                    AirlineId = airline.AirlineId,
                    City = airline.Address.City,
                    State = airline.Address.State,
                    Lat = airline.Address.Lat,
                    Lon = airline.Address.Lon,
                    Name = airline.Name,
                    Logo = airline.LogoUrl,
                    About = airline.PromoDescription,
                    Destinations = allDest,
                    rate = rate
                };

                return Ok(obj);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return airline");
            }
        }

        [HttpGet]
        [Route("airline-special-offers/{id}")]
        public async Task<IActionResult> AirlineSpecialOffers(int id)
        {
            try
            {
                var airline = await unitOfWork.AirlineRepository.GetByID(id);

                if (airline == null)
                {
                    return NotFound("Airline not found");
                }

                var offers = await unitOfWork.SpecialOfferRepository.GetSpecialOffersOfAirline(airline);

                List<object> objs = new List<object>();
                List<object> flights = new List<object>();
                List<object> fstops = new List<object>();
                bool passed = true;

                foreach (var item in offers)
                {

                    flights = new List<object>();

                    foreach (var seat in item.Seats)
                    {
                        if (seat.Flight.TakeOffDateTime < DateTime.Now.Date)
                        {
                            passed = false;
                            continue;
                        }

                        fstops = new List<object>();

                        foreach (var stop in seat.Flight.Stops)
                        {
                            fstops.Add(new
                            {
                                city = stop.Destination.City
                            });
                        }


                        flights.Add(new
                        {
                            flightId = seat.Flight.FlightId,
                            flightNumber = seat.Flight.FlightNumber,
                            to = seat.Flight.To.City,
                            from = seat.Flight.From.City,
                            tripLength = seat.Flight.tripLength,
                            tripTime = seat.Flight.TripTime,
                            stops = fstops,
                            landingDate = seat.Flight.LandingDateTime.Date,
                            landingTime = seat.Flight.LandingDateTime.TimeOfDay,
                            takeOffDate = seat.Flight.TakeOffDateTime.Date,
                            takeOffTime = seat.Flight.TakeOffDateTime.TimeOfDay,
                            seat.Class,
                            seat.Column,
                            seat.Row,
                            seat.Price,
                            seat.Reserved,
                            seat.SeatId
                        }
                        );
                    }
                    if (passed)
                    {
                        objs.Add(new { airline.LogoUrl, airline.Name, item.NewPrice, item.OldPrice, item.SpecialOfferId, flights });
                    }
                }

                return Ok(objs);

            }
            catch (Exception)
            {

                return StatusCode(500, "Failed to return special offers");
            }
        }

        [HttpGet]
        [Route("flights")]
        public async Task<IActionResult> Flights()
        {
            try
            {
                var queryString = Request.Query;
                var tripType = queryString["type"].ToString();
                var y = queryString["dep"].ToString();
                var t = queryString["dep"].ToString().Split(',');

                List<DateTime> departures = new List<DateTime>();
                if (!String.IsNullOrEmpty(queryString["dep"].ToString()))
                {
                    foreach (var item in queryString["dep"].ToString().Split(','))
                    {
                        if (Convert.ToDateTime(item) < DateTime.Now)
                        {
                            return BadRequest("Selected date is in past");
                        }
                        departures.Add(Convert.ToDateTime(item));
                    }
                }

                List<string> fromList = new List<string>();
                List<string> toList = new List<string>();
                string from = "", to = "";

                if (tripType == "multi")
                {
                    foreach (var item in queryString["from"].ToString().Split(','))
                    {
                        fromList.Add(item);
                    }
                    foreach (var item in queryString["to"].ToString().Split(','))
                    {
                        toList.Add(item);
                    }
                }
                else
                {
                    from = queryString["from"].ToString();
                    to = queryString["to"].ToString();
                }


                DateTime ret = DateTime.MinValue;
                if (!String.IsNullOrEmpty(queryString["ret"].ToString()))
                {
                    ret = Convert.ToDateTime(queryString["ret"].ToString());
                }
                float minPrice = 0;
                float maxPrice = 3000;

                float.TryParse(queryString["minprice"], out minPrice);
                float.TryParse(queryString["maxprice"], out maxPrice);

                List<int> ids = new List<int>();
                if (!String.IsNullOrEmpty(queryString["air"].ToString()))
                {
                    foreach (var item in queryString["air"].ToString().Split(','))
                    {
                        ids.Add(int.Parse(item));
                    }
                }

                var minDuration = queryString["mind"].ToString();
                var maxDuration = queryString["maxd"].ToString();

                var flights = await unitOfWork.FlightRepository
                                                .GetAllFlightsWithAllProp(f => f.TakeOffDateTime >= DateTime.Now
                                                                        && tripType.Equals("one") ? (f.From.City.Equals(from) && f.To.City.Equals(to) && departures.Contains(f.TakeOffDateTime.Date)) : true
                                                                        && tripType.Equals("two") ? (f.From.City.Equals(from) && f.To.City.Equals(to) && departures.Contains(f.TakeOffDateTime.Date)) || (f.From.City.Equals(to) && f.To.City.Equals(from) && ret.Equals(f.TakeOffDateTime.Date)) : true
                                                                        );

                ICollection<object> flightsObject = new List<object>();
                ICollection<object> twoWayFlights = new List<object>();
                ICollection<object> oneWayFlights = new List<object>();

                ICollection<object> multiWayFlights = new List<object>();



                if (tripType == "one")
                {
                    foreach (var flight in flights)
                    {
                        if (flight.TakeOffDateTime < DateTime.Now)
                        {
                            continue;
                        }

                        if (!(await FilterFromPassed(flight, to, from, ids, minDuration, maxDuration, departures, minPrice, maxPrice)))
                        {
                            continue;
                        }

                        List<object> stops = new List<object>();

                        if (flight.Stops != null)
                        {
                            foreach (var stop in flight.Stops)
                            {
                                //var s = await unitOfWork.AirlineRepository.GetDestination(stop.DestinationId);
                                //stops.Add(new { s.City });
                                stops.Add(new { stop.Destination.City, stop.Destination.State });

                            }
                        }


                        flightsObject.Add(new
                        {
                            takeOffDate = flight.TakeOffDateTime.Date,
                            landingDate = flight.LandingDateTime.Date,
                            airlineLogo = flight.Airline.LogoUrl,
                            airlineName = flight.Airline.Name,
                            airlineId = flight.Airline.AirlineId,
                            from = flight.From.City,
                            to = flight.To.City,
                            takeOffTime = flight.TakeOffDateTime.TimeOfDay,
                            landingTime = flight.LandingDateTime.TimeOfDay,
                            flightTime = flight.TripTime,
                            flightLength = flight.tripLength,
                            flightNumber = flight.FlightNumber,
                            flightId = flight.FlightId,
                            stops = stops,
                            minPrice = flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price
                        });
                        oneWayFlights.Add(new { flightsObject });
                    }
                    return Ok(oneWayFlights);
                }

                else if (tripType == "two")
                {
                    foreach (var flight in flights)
                    {
                        if (flight.TakeOffDateTime < DateTime.Now.Date)
                        {
                            continue;
                        }
                        if (!(await FilterFromPassed(flight, to, from, ids, minDuration, maxDuration, departures, minPrice, maxPrice)))
                        {
                            continue;
                        }

                        List<object> stops = new List<object>();

                        if (flight.Stops != null)
                        {
                            foreach (var stop in flight.Stops)
                            {
                                //var s = await unitOfWork.AirlineRepository.GetDestination(stop.DestinationId);
                                //stops.Add(new { s.City });
                                stops.Add(new { stop.Destination.City, stop.Destination.State });

                            }
                        }


                        flightsObject.Add(new
                        {
                            takeOffDate = flight.TakeOffDateTime.Date,
                            landingDate = flight.LandingDateTime.Date,
                            airlineLogo = flight.Airline.LogoUrl,
                            airlineName = flight.Airline.Name,
                            airlineId = flight.Airline.AirlineId,
                            from = flight.From.City,
                            to = flight.To.City,
                            takeOffTime = flight.TakeOffDateTime.TimeOfDay,
                            landingTime = flight.LandingDateTime.TimeOfDay,
                            flightTime = flight.TripTime,
                            flightLength = flight.tripLength,
                            flightNumber = flight.FlightNumber,
                            flightId = flight.FlightId,
                            stops = stops,
                            minPrice = flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price
                        });

                        foreach (var returnFlights in flights)
                        {
                            if (returnFlights.TakeOffDateTime < DateTime.Now.Date)
                            {
                                continue;
                            }
                            if (!(await FilterReturnPassed(returnFlights, to, from, ids, minDuration, maxDuration, ret, minPrice, maxPrice, flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price, flight.Seats.OrderByDescending(s => s.Price).FirstOrDefault().Price)))
                            {
                                continue;
                            }

                            stops = new List<object>();

                            if (returnFlights.Stops != null)
                            {
                                foreach (var stop in returnFlights.Stops)
                                {
                                    stops.Add(new { stop.Destination.City, stop.Destination.State });
                                }
                            }


                            flightsObject.Add(new
                            {
                                takeOffDate = returnFlights.TakeOffDateTime.Date,
                                landingDate = returnFlights.LandingDateTime.Date,
                                airlineLogo = returnFlights.Airline.LogoUrl,
                                airlineName = returnFlights.Airline.Name,
                                airlineId = returnFlights.Airline.AirlineId,
                                from = returnFlights.From.City,
                                to = returnFlights.To.City,
                                takeOffTime = returnFlights.TakeOffDateTime.TimeOfDay,
                                landingTime = returnFlights.LandingDateTime.TimeOfDay,
                                flightTime = returnFlights.TripTime,
                                flightLength = returnFlights.tripLength,
                                flightNumber = returnFlights.FlightNumber,
                                flightId = returnFlights.FlightId,
                                stops = stops,
                                minPrice = returnFlights.Seats.OrderBy(s => s.Price).FirstOrDefault().Price
                            });

                            twoWayFlights.Add(new { flightsObject });
                            flightsObject = new List<object>();
                            flightsObject.Add(new { flight });

                        }

                    }
                    return Ok(twoWayFlights);
                }
                else
                {
                    float currentPrice = 0;

                    foreach (var flight in flights)
                    {
                        if (flight.TakeOffDateTime < DateTime.Now.Date)
                        {
                            continue;
                        }
                        List<string> tempFrom = fromList;
                        List<string> tempTo = toList;
                        List<DateTime> tempDepartures = departures;
                        currentPrice = 0;

                        if (!(await FilterMultiPassed(flight, fromList, toList, ids, minDuration, maxDuration, departures, minPrice, maxPrice, currentPrice)))
                        {
                            continue;
                        }

                        currentPrice += flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price;

                        List<object> stops = new List<object>();

                        if (flight.Stops != null)
                        {
                            foreach (var stop in flight.Stops)
                            {
                                stops.Add(new { stop.Destination.City, stop.Destination.State });
                            }
                        }


                        flightsObject.Add(new
                        {
                            takeOffDate = flight.TakeOffDateTime.Date,
                            landingDate = flight.LandingDateTime.Date,
                            airlineLogo = flight.Airline.LogoUrl,
                            airlineName = flight.Airline.Name,
                            airlineId = flight.Airline.AirlineId,
                            from = flight.From.City,
                            to = flight.To.City,
                            takeOffTime = flight.TakeOffDateTime.TimeOfDay,
                            landingTime = flight.LandingDateTime.TimeOfDay,
                            flightTime = flight.TripTime,
                            flightLength = flight.tripLength,
                            flightNumber = flight.FlightNumber,
                            flightId = flight.FlightId,
                            stops = stops,
                            minPrice = flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price
                        });

                        tempFrom.Remove(flight.From.City);
                        tempTo.Remove(flight.To.City);
                        tempDepartures.Remove(flight.TakeOffDateTime.Date);

                        foreach (var returnFlights in flights)
                        {
                            if (returnFlights.TakeOffDateTime < DateTime.Now.Date)
                            {
                                continue;
                            }
                            if (tempFrom.Count == 0)
                            {
                                break;
                            }
                            if (!(await FilterMultiPassed(returnFlights, tempFrom, tempTo, ids, minDuration, maxDuration, tempDepartures, minPrice, maxPrice, currentPrice)))
                            {


                                continue;
                            }
                            currentPrice += returnFlights.Seats.OrderBy(s => s.Price).FirstOrDefault().Price;

                            tempFrom.Remove(returnFlights.From.City);
                            tempTo.Remove(returnFlights.To.City);
                            tempDepartures.Remove(returnFlights.TakeOffDateTime.Date);

                            stops = new List<object>();

                            if (returnFlights.Stops != null)
                            {
                                foreach (var stop in returnFlights.Stops)
                                {
                                    stops.Add(new { stop.Destination.City, stop.Destination.State });
                                }
                            }


                            flightsObject.Add(new
                            {
                                takeOffDate = returnFlights.TakeOffDateTime.Date,
                                landingDate = returnFlights.LandingDateTime.Date,
                                airlineLogo = returnFlights.Airline.LogoUrl,
                                airlineName = returnFlights.Airline.Name,
                                airlineId = returnFlights.Airline.AirlineId,
                                from = returnFlights.From.City,
                                to = returnFlights.To.City,
                                takeOffTime = returnFlights.TakeOffDateTime.TimeOfDay,
                                landingTime = returnFlights.LandingDateTime.TimeOfDay,
                                flightTime = returnFlights.TripTime,
                                flightLength = returnFlights.tripLength,
                                flightNumber = returnFlights.FlightNumber,
                                flightId = returnFlights.FlightId,
                                stops = stops,
                                minPrice = returnFlights.Seats.OrderBy(s => s.Price).FirstOrDefault().Price
                            });

                            multiWayFlights.Add(new { flightsObject });
                            flightsObject = new List<object>();
                            flightsObject.Add(new { flight });

                        }

                    }
                    if (multiWayFlights.Count == fromList.Count)
                    {
                        return Ok(multiWayFlights);
                    }
                    return Ok();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return flights");
            }
        }

        [HttpGet]
        [Route("flight-seats")]
        public async Task<IActionResult> FlightSeats()
        {
            try
            {
                var queryString = Request.Query;
                var idsOfFlights = queryString["ids"].ToList();

                var flights = await unitOfWork.FlightRepository.GetFlights(idsOfFlights);

                if (flights == null || flights.ToArray().Length == 0)
                {
                    return NotFound("Flights not found");
                }

                var retVal = new List<object>();
                List<object> obj = new List<object>();
                var stops = new List<object>();

                foreach (var flight in flights)
                {

                    stops = new List<object>();

                    foreach (var item in flight.Stops)
                    {
                        stops.Add(new
                        {
                            City = item.Destination.City,
                            State = item.Destination.State,
                        });
                    }
                    obj = new List<object>();
                    foreach (var item in flight.Seats)
                    {
                        obj.Add(new
                        {
                            item.Column,
                            item.Row,
                            flight.FlightId,
                            item.Class,
                            item.SeatId,
                            item.Price,
                            available = item.Available
                        });
                    }

                    retVal.Add(new
                    {
                        seats = obj.ToArray(),
                        takeOffDate = flight.TakeOffDateTime.Date,
                        landingDate = flight.LandingDateTime.Date,
                        from = flight.From.City,
                        to = flight.To.City,
                        takeOffTime = flight.TakeOffDateTime.TimeOfDay,
                        landingTime = flight.LandingDateTime.TimeOfDay,
                        flightTime = flight.TripTime,
                        flightLength = flight.tripLength,
                        flightNumber = flight.FlightNumber,
                        flightId = flight.FlightId,
                        stops = stops,
                        airlineLogo = flight.Airline.LogoUrl,
                        airlineName = flight.Airline.Name
                    });
                }

                return Ok(retVal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Failed to return flight and seats");
            }
        }

        [HttpGet]
        [Route("all-airlines-specialoffers")]
        public async Task<IActionResult> AllSpecOffers()
        {
            try
            {
                var specOffers = await unitOfWork.SpecialOfferRepository.GetAllSpecOffers();

                List<object> objs = new List<object>();
                List<object> flights = new List<object>();
                List<object> fstops = new List<object>();

                bool passed = true;
                foreach (var item in specOffers)
                {
                    if (item.IsReserved)
                    {
                        continue;
                    }
                    flights = new List<object>();
                    passed = true;

                    foreach (var seat in item.Seats)
                    {
                        if (seat.Flight.TakeOffDateTime <= DateTime.Now)
                        {
                            passed = false;
                            break;
                        }

                        fstops = new List<object>();

                        foreach (var stop in seat.Flight.Stops)
                        {
                            fstops.Add(new
                            {
                                city = stop.Destination.City
                            });
                        }

                        flights.Add(new
                        {
                            flightId = seat.Flight.FlightId,
                            flightNumber = seat.Flight.FlightNumber,
                            to = seat.Flight.To.City,
                            from = seat.Flight.From.City,
                            airlineName = seat.Flight.Airline.Name,
                            airlineLogo = seat.Flight.Airline.LogoUrl,
                            tripLength = seat.Flight.tripLength,
                            tripTime = seat.Flight.TripTime,
                            stops = fstops,
                            landingDate = seat.Flight.LandingDateTime.Date,
                            landingTime = seat.Flight.LandingDateTime.TimeOfDay,
                            takeOffDate = seat.Flight.TakeOffDateTime.Date,
                            takeOffTime = seat.Flight.TakeOffDateTime.TimeOfDay,
                            seat.Class,
                            seat.Column,
                            seat.Row,
                            seat.Price,
                            seat.Reserved,
                            seat.SeatId
                        }
                        );
                    }
                    if (passed)
                    {
                        objs.Add(
                        new
                        {
                            item.Airline.Name,
                            item.Airline.LogoUrl,
                            item.NewPrice,
                            item.OldPrice,
                            item.SpecialOfferId,
                            flights
                        });
                    }
                }

                return Ok(objs);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to return special offers");
            }
        }


        private async Task<bool> FilterPassed(Flight flight, string tripType, string to, string from,
            List<string> fromMulti, List<string> toMulti, List<int> airlines, DateTime returnDate,
            string minDuration, string maxDuration, List<DateTime> departures, float minPrice, float maxPrice)
        {
            await Task.Yield();

            switch (tripType)
            {
                case "one":
                    if (!flight.From.City.Equals(from) || !flight.To.City.Equals(to))
                    {
                        return false;
                    }
                    if (airlines.Count > 0)
                    {
                        if (!airlines.Contains(flight.AirlineId))
                        {
                            return false;
                        }
                    }
                    if (departures[0].Date != flight.TakeOffDateTime.Date)
                    {
                        return false;
                    }
                    if (minPrice > flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price || flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price > maxPrice)
                    {
                        return false;
                    }
                    if (!String.IsNullOrEmpty(minDuration) || !String.IsNullOrEmpty(maxDuration))
                    {
                        if (int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) < int.Parse(minDuration.Split('h', ' ', 'm')[0])
                            || int.Parse(flight.TripTime.Split('h', ' ', 'm')[1]) < int.Parse(minDuration.Split('h', ' ', 'm')[1])
                            || int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) > int.Parse(maxDuration.Split('h', ' ', 'm')[0])
                            || int.Parse(flight.TripTime.Split('h', ' ', 'm')[1]) > int.Parse(maxDuration.Split('h', ' ', 'm')[1]))
                        {
                            return false;
                        }
                    }
                    return true;

                case "two":
                    bool passed = false;
                    if (flight.From.City.Equals(from) && flight.To.City.Equals(to) && departures[0].Date == flight.TakeOffDateTime.Date)
                    {
                        passed = true;
                    }
                    if (flight.To.City.Equals(from) && flight.From.City.Equals(to) && returnDate.Date == flight.TakeOffDateTime.Date)
                    {
                        passed = true;
                    }

                    if (!passed)
                    {
                        return false;
                    }

                    if (airlines.Count > 0)
                    {
                        if (!airlines.Contains(flight.AirlineId))
                        {
                            return false;
                        }
                    }

                    if (minPrice > flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price || flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price > maxPrice)
                    {
                        return false;
                    }
                    if (int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) < int.Parse(minDuration.Split('h', ' ', 'm')[0])
                        || int.Parse(flight.TripTime.Split('h', ' ', 'm')[1]) < int.Parse(minDuration.Split('h', ' ', 'm')[1])
                        || int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) > int.Parse(maxDuration.Split('h', ' ', 'm')[0])
                        || int.Parse(flight.TripTime.Split('h', ' ', 'm')[1]) > int.Parse(maxDuration.Split('h', ' ', 'm')[1]))
                    {
                        return false;
                    }

                    return true;

                case "multi":

                    bool pass = false;

                    for (int i = 0; i < fromMulti.Count; i++)
                    {
                        if (flight.From.City.Equals(fromMulti[i]) && flight.To.City.Equals(toMulti[i]) && departures[i].Date == flight.TakeOffDateTime.Date)
                        {
                            pass = true;
                            break;
                        }
                    }
                    if (!pass)
                    {
                        return false;
                    }
                    if (airlines.Count > 0)
                    {
                        if (!airlines.Contains(flight.AirlineId))
                        {
                            return false;
                        }
                    }
                    if (minPrice > flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price || flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price > maxPrice)
                    {
                        return false;
                    }
                    if (int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) < int.Parse(minDuration.Split('h', ' ', 'm')[0])
                        || int.Parse(flight.TripTime.Split('h', ' ', 'm')[1]) < int.Parse(minDuration.Split('h', ' ', 'm')[1])
                        || int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) > int.Parse(maxDuration.Split('h', ' ', 'm')[0])
                        || int.Parse(flight.TripTime.Split('h', ' ', 'm')[1]) > int.Parse(maxDuration.Split('h', ' ', 'm')[1]))
                    {
                        return false;
                    }

                    return true;

                default:
                    break;
            }

            return false;
        }


        private async Task<bool> FilterFromPassed(Flight flight, string to, string from, List<int> airlines,
            string minDuration, string maxDuration, List<DateTime> departures, float minPrice, float maxPrice)
        {
            await Task.Yield();

            //if (!flight.From.City.Equals(from) || !flight.To.City.Equals(to))
            //{
            //    return false;
            //}
            if (airlines.Count > 0)
            {
                if (!airlines.Contains(flight.AirlineId))
                {
                    return false;
                }
            }
            //if (departures[0].Date != flight.TakeOffDateTime.Date)
            //{
            //    return false;
            //}
            if (minPrice > flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price || flight.Seats.OrderByDescending(s => s.Price).FirstOrDefault().Price > maxPrice)
            {
                return false;
            }
            if (!String.IsNullOrEmpty(minDuration) || !String.IsNullOrEmpty(maxDuration))
            {
                var flightTime = new DateTime(1, 1, 1, int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]), int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]), 0);
                var fliterTimeMin = new DateTime(1, 1, 1, int.Parse(minDuration.Split('h', ' ', 'm')[0]), int.Parse(minDuration.Split('h', ' ', 'm')[2]), 0);
                var fliterTimeMax = new DateTime(1, 1, 1, int.Parse(maxDuration.Split('h', ' ', 'm')[0]), int.Parse(maxDuration.Split('h', ' ', 'm')[2]), 0);

                if (flightTime > fliterTimeMax || flightTime < fliterTimeMin)
                {
                    return false;
                }
                //if (int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) < int.Parse(minDuration.Split('h', ' ', 'm')[0])
                //    || int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]) < int.Parse(minDuration.Split('h', ' ', 'm')[2])
                //    || int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) > int.Parse(maxDuration.Split('h', ' ', 'm')[0])
                //    || int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]) > int.Parse(maxDuration.Split('h', ' ', 'm')[2]))
                //{
                //    return false;
                //}
            }
            return true;
        }

        private async Task<bool> FilterReturnPassed(Flight flight, string from, string to, List<int> airlines,
          string minDuration, string maxDuration, DateTime departure, float minPrice, float maxPrice, float firstflightMin, float firstFlightMax)
        {
            await Task.Yield();

            //if (!flight.From.City.Equals(from) || !flight.To.City.Equals(to))
            //{
            //    return false;
            //}
            if (airlines.Count > 0)
            {
                if (!airlines.Contains(flight.AirlineId))
                {
                    return false;
                }
            }
            //if (departure.Date != flight.TakeOffDateTime.Date)
            //{
            //    return false;
            //}
            if (minPrice > firstflightMin + flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price || flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price + firstFlightMax > maxPrice)
            {
                return false;
            }
            if (!String.IsNullOrEmpty(minDuration) || !String.IsNullOrEmpty(maxDuration))
            {
                if (int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) < int.Parse(minDuration.Split('h', ' ', 'm')[0])
                    || int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]) < int.Parse(minDuration.Split('h', ' ', 'm')[2])
                    || int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) > int.Parse(maxDuration.Split('h', ' ', 'm')[0])
                    || int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]) > int.Parse(maxDuration.Split('h', ' ', 'm')[2]))
                {
                    return false;
                }
            }
            return true;
        }


        private async Task<bool> FilterMultiPassed(Flight flight, List<string> fromMulti, List<string> toMulti, List<int> airlines,
           string minDuration, string maxDuration, List<DateTime> departures, float minPrice, float maxPrice, float currentFlightsPrice)
        {
            await Task.Yield();

            if (fromMulti.Contains(flight.From.City) && toMulti.Contains(flight.To.City))
            {
            }
            else
            {
                return false;
            }

            if (!departures.Contains(flight.TakeOffDateTime.Date))
            {
                return false;
            }

            if (airlines.Count > 0)
            {
                if (!airlines.Contains(flight.AirlineId))
                {
                    return false;
                }
            }
            if (minPrice > flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price + currentFlightsPrice
                || flight.Seats.OrderBy(s => s.Price).FirstOrDefault().Price + currentFlightsPrice > maxPrice)
            {
                return false;
            }
            if (int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) < int.Parse(minDuration.Split('h', ' ', 'm')[0])
                || int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]) < int.Parse(minDuration.Split('h', ' ', 'm')[2])
                || int.Parse(flight.TripTime.Split('h', ' ', 'm')[0]) > int.Parse(maxDuration.Split('h', ' ', 'm')[0])
                || int.Parse(flight.TripTime.Split('h', ' ', 'm')[2]) > int.Parse(maxDuration.Split('h', ' ', 'm')[2]))
            {
                return false;
            }

            return true;
        }
    }
}
