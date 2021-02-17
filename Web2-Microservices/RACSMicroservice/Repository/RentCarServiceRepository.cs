using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RACSMicroservice.Data;
using RACSMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public class RentCarServiceRepository : GenericRepository<RentACarService>, IRentCarServiceRepository
    {
        public RentCarServiceRepository(RACSContext context) : base(context)
        {
        }

        public async Task<IdentityResult> UpdateAddress(Address addr)
        {
            context.Entry(addr).State = EntityState.Modified;
            var res = await context.SaveChangesAsync();
            if (res > 0)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(new IdentityError() { Code = "Update error" });
        }

        public async Task<RentACarService> GetRACSAndCars(string adminId)  // kupi i od filijala auta
        {
            return await context.RentACarServices
                .Include(r => r.Cars)
                .Include(r => r.Address)
                .Include(r => r.Branches)
                .ThenInclude(b => b.Cars)
                .ThenInclude(c => c.Rates)
                .FirstOrDefaultAsync(r => r.AdminId == adminId);
        }

        public async Task<RentACarService> GetRacsWithRates(int racsId)
        {
            return await context.RentACarServices
                .Include(r => r.Rates)
                .FirstOrDefaultAsync(racs => racs.RentACarServiceId == racsId);
        }
    }
}
