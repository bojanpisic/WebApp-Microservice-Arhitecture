using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AirlineContext context) : base(context)
        {
        }

        public async Task<Ticket> GetTicket(int ticketId)
        {
            return await context.Tickets
                .Include(t => t.Seat)
                .ThenInclude(s => s.Flight)
                .ThenInclude(f => f.Airline)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }
    }
}