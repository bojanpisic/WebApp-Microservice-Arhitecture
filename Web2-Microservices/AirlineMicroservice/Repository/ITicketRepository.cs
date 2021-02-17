using AirlineMicroservice.Models;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetTicket(int ticketId);
    }
}