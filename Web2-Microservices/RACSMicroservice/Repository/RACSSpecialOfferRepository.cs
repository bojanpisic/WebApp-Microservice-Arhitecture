using Microsoft.EntityFrameworkCore;
using RACSMicroservice.Data;
using RACSMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public class RACSSpecialOfferRepository : GenericRepository<CarSpecialOffer>, IRACSSpecialOffer
    {
        public RACSSpecialOfferRepository(RACSContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CarSpecialOffer>> GetSpecialOffersOfRacs(int racsId)
        {
            return await context.CarSpecialOffers.Include(s => s.Car)
                .ThenInclude(c => c.Branch)
                .Include(s => s.Car)
                .ThenInclude(c => c.RentACarService)
                .Where(spec => spec.Car.Branch.RentACarServiceId == racsId || spec.Car.RentACarServiceId == racsId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CarSpecialOffer>> GetSpecialOffersOfAllRacs()
        {
            return await context.CarSpecialOffers
                .Include(s => s.Car)
                .ThenInclude(c => c.Branch)
                .Include(s => s.Car)
                .ThenInclude(c => c.RentACarService)
                .ToListAsync();
        }

        public async Task<CarSpecialOffer> GetSpecialOfferById(int specialOfferId)
        {
            return await context.CarSpecialOffers
                .Include(s => s.Car)
                .ThenInclude(c => c.Branch)
                .Include(s => s.Car)
                .ThenInclude(c => c.RentACarService)
                .ThenInclude(r => r.Address)
                .FirstOrDefaultAsync(spec => spec.CarSpecialOfferId == specialOfferId);
        }

    }
}
