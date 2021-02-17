using AirlineMicroservice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface IDestinationRepository : IGenericRepository<Destination>
    {
        Task<IEnumerable<Destination>> GetAirlineDestinations(Airline airline);

    }
}