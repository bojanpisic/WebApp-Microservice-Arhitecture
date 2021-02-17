using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class FlightReservationRepository : GenericRepository<FlightReservation>, IFlightReservationRepository
    {
        public FlightReservationRepository(AirlineContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FlightReservation>> GetTrips(string userId)
        {
            return await context.FlightReservations
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Seat)
                    .ThenInclude(s => s.Flight)
                    .ThenInclude(f => f.From)
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Seat)
                    .ThenInclude(s => s.Flight)
                    .ThenInclude(f => f.To)
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Seat)
                    .ThenInclude(s => s.Flight)
                    .ThenInclude(ff => ff.Airline)
                    .ThenInclude(a => a.Rates)
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Seat)
                    .ThenInclude(s => s.Flight)
                    .ThenInclude(f => f.Stops)
                    .ThenInclude(d => d.Destination)
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Seat)
                    .ThenInclude(s => s.Flight)
                    .ThenInclude(f => f.Rates)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<FlightReservation> GetReservationById(int flightReservationId)
        {
            return await context.FlightReservations
                .Include(f => f.Tickets)
                .ThenInclude(t => t.Seat)
                .ThenInclude(s => s.Flight)
                //.Include(t => t.CarRent)
                //.ThenInclude(c => c.RentedCar)
                .FirstOrDefaultAsync(t => t.FlightReservationId == flightReservationId);
        }
    }
}
