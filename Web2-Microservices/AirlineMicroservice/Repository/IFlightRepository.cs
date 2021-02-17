using AirlineMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        Task<IEnumerable<Flight>> GetAirlineFlights(int airlineId);
        Task<IEnumerable<Flight>> GetAllFlightsWithAllProp(Expression<Func<Flight, bool>> filter = null);
        Task<IEnumerable<Flight>> GetFlights(List<string> ids);

    }
}