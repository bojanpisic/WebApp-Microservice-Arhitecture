using Microsoft.AspNetCore.Identity;
using RACSMicroservice.Models;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public interface IRentCarServiceRepository : IGenericRepository<RentACarService>
    {
        Task<RentACarService> GetRACSAndCars(string adminId);
        Task<IdentityResult> UpdateAddress(Address addr);
        Task<RentACarService> GetRacsWithRates(int racsId);
    }
}