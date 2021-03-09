using AirlineMicroservice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        Task<Seat> GetSeat(int seatId);
        Task<IEnumerable<Seat>> GetSeats(List<int> seatsIds);
        Task<Seat> GetSeatWithTicket(int seatId);
    }
}