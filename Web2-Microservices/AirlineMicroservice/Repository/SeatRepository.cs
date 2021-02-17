using AirlineMicroservice.Data;
using AirlineMicroservice.Models;

namespace AirlineMicroservice.Repository
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(AirlineContext context) : base(context)
        {
        }
    }
}