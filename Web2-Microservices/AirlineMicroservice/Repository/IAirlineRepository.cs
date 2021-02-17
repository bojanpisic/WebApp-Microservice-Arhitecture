using AirlineMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface IAirlineRepository : IGenericRepository<Airline>
    {
        Task<IEnumerable<Airline>> GetAllAirlines();
        void UpdateAddress(Address addr);
        Task<Airline> GetAirline(int id);
    }
}
