using Microsoft.EntityFrameworkCore;
using RACSMicroservice.Data;
using RACSMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(RACSContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Car>> AllCars(Expression<Func<Car, bool>> filter = null)
        {
            return await context.Cars
                .Include(c => c.SpecialOffers)
                .Include(c => c.Branch)
                    .ThenInclude(b => b.RentACarService)
                        .ThenInclude(r => r.Address)
                .Include(c => c.Branch)
                    .ThenInclude(b => b.RentACarService)
                        .ThenInclude(r => r.Branches)
                .Include(c => c.RentACarService)
                    .ThenInclude(r => r.Address)
                .Include(c => c.RentACarService)
                    .ThenInclude(r => r.Branches)
                .Include(c => c.Rates)
                .Include(c => c.Rents)
                .Where(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> CarsOfBranch(int id)
        {
            return await context.Cars
                .Include(c => c.Branch)
                    .ThenInclude(b => b.RentACarService)
                        .ThenInclude(r => r.Address)
                .Include(c => c.Rates)
                .Where(c => c.BranchId == id)
                .ToListAsync();
        }
    }
}
