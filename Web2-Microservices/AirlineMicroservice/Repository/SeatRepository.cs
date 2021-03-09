using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(AirlineContext context) : base(context)
        {
        }
        public async Task<Seat> GetSeatWithTicket(int seatId)
        {
            return await context.Seats
                .Include(s => s.Flight)
                .Include(s => s.Ticket)
                .FirstAsync(s => s.SeatId.Equals(seatId));
        }

        public async Task<Seat> GetSeat(int seatId)
        {
            return await context.Seats
                .Include(s => s.Flight)
                    .ThenInclude(s => s.Airline)
                .Include(s => s.Flight)
                    .ThenInclude(s => s.From)
                .Include(s => s.Flight)
                    .ThenInclude(s => s.To)
                .FirstAsync(s => s.SeatId.Equals(seatId));
        }

        public async Task<IEnumerable<Seat>> GetSeats(List<int> seatsIds)
        {
            return await context.Seats
                .Include(s => s.Flight)
                    .ThenInclude(s => s.Airline)
                .Include(s => s.Flight)
                    .ThenInclude(s => s.From)
                .Include(s => s.Flight)
                    .ThenInclude(s => s.To)
                .Where(s => seatsIds.Contains(s.SeatId))
                .ToListAsync();
        }
    }
}