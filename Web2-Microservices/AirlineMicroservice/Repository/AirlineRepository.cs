using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class AirlineRepository : GenericRepository<Airline>, IAirlineRepository
    {
        public AirlineRepository(AirlineContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Airline>> GetAllAirlines()
        {
            return await context.Airlines
                .Include(a => a.Destinations)
                .ThenInclude(d => d.Destination)
                .Include(a => a.Rates)
                .Include(a => a.Address)
                .ToListAsync();
        }

        public void UpdateAddress(Address addr)
        {
            context.Entry(addr).State = EntityState.Modified;
        }

        public async Task<Airline> GetAirline(int id)
        {
            return await context.Airlines
                .Include(a => a.Destinations)
                .ThenInclude(d => d.Destination)
                .Include(a => a.Address)
                .Include(a => a.Rates)
                .FirstOrDefaultAsync(a => a.AirlineId == id);
        }
    }
}
