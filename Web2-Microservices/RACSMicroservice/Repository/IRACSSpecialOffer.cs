using RACSMicroservice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public interface IRACSSpecialOffer : IGenericRepository<CarSpecialOffer>
    {
        Task<IEnumerable<CarSpecialOffer>> GetSpecialOffersOfRacs(int racsId);
        Task<IEnumerable<CarSpecialOffer>> GetSpecialOffersOfAllRacs();
        Task<CarSpecialOffer> GetSpecialOfferById(int specialOfferId);
    }
}