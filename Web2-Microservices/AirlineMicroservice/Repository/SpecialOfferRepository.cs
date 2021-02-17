using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class SpecialOfferRepository : GenericRepository<SpecialOffer>, ISpecialOfferRepository
    {
        public SpecialOfferRepository(AirlineContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SpecialOffer>> GetAllSpecOffers()
        {
            return await context.SpecialOffers
              .Include(s => s.Seats)
              .ThenInclude(seat => seat.Flight)
              .ThenInclude(to => to.To)
              .Include(s => s.Seats)
              .ThenInclude(seat => seat.Flight)
              .ThenInclude(from => from.From)
              .Include(s => s.Seats)
              .ThenInclude(seat => seat.Flight)
              .ThenInclude(h => h.Stops)
              .ThenInclude(st => st.Destination)
              .Include(s => s.Seats)
              .ThenInclude(seat => seat.Flight)
              .ThenInclude(f => f.Airline)
              .ToListAsync();
        }

        public async Task<IEnumerable<SpecialOffer>> GetSpecialOffersOfAirline(Airline airline)
        {
            return await context.SpecialOffers.Include(s => s.Seats)
                .ThenInclude(seat => seat.Flight)
                .ThenInclude(to => to.To)
                .Include(s => s.Seats)
                .ThenInclude(seat => seat.Flight)
                .ThenInclude(from => from.From)
                .Include(s => s.Seats)
                .ThenInclude(seat => seat.Flight)
                .ThenInclude(h => h.Stops)
                .ThenInclude(st => st.Destination)
                .Where(s => s.Airline == airline).ToListAsync();
        }
        public async Task<SpecialOffer> GetSpecialOfferById(int id)
        {
            return await context.SpecialOffers
                .Include(s => s.Seats)
                .ThenInclude(seat => seat.Flight)
                .FirstOrDefaultAsync(s => s.SpecialOfferId == id);
        }

    }
}