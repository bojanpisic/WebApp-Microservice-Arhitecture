using AirlineMicroservice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface ISpecialOfferRepository : IGenericRepository<SpecialOffer>
    {
        Task<IEnumerable<SpecialOffer>> GetAllSpecOffers();
        Task<IEnumerable<SpecialOffer>> GetSpecialOffersOfAirline(Airline airline);
        Task<SpecialOffer> GetSpecialOfferById(int id);
    }
}