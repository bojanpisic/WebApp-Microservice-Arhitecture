using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        public FlightRepository(AirlineContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Flight>> GetAirlineFlights(int airlineId)
        {
            return await context.Flights
                .Where(f => f.AirlineId == airlineId)
                .Include(f => f.Seats)
                .Include(f => f.From)
                .Include(f => f.To)
                .Include(f => f.Airline)
                .Include(f => f.Stops)
                .ThenInclude(d => d.Destination)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsWithAllProp(Expression<Func<Flight, bool>> filter = null)
        {
            return await context.Flights
                .Include(f => f.Seats)
                .Include(f => f.From)
                .Include(f => f.To)
                .Include(f => f.Airline)
                .Include(f => f.Stops)
                .ThenInclude(d => d.Destination)
                //.Where(f => f.TakeOffDateTime >= DateTime.Now)
                .Where(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flight>> GetFlights(List<string> ids)
        {
            return await context.Flights
              .Include(f => f.Seats)
              .Include(f => f.From)
              .Include(f => f.To)
              .Include(f => f.Airline)
              .Include(f => f.Stops)
              .ThenInclude(d => d.Destination)
              .Where(f => ids.Contains(f.FlightId.ToString()) && f.TakeOffDateTime >= DateTime.Now)
              .ToListAsync();
        }
    }
}