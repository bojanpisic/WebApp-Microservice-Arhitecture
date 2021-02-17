using AirlineMicroservice.Data;
using AirlineMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class DestinationRepository : GenericRepository<Destination>, IDestinationRepository
    {
        public DestinationRepository(AirlineContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Destination>> GetAirlineDestinations(Airline airline)
        {
            //return await context.Destinations.Where(d => d.Airlines.Any( a=> a.Airline == airline)).ToListAsync(); 
            return await context.AirlineDestination
                .Where(ad => ad.AirlineId == airline.AirlineId)
                .Select(ad => ad.Destination)
                .ToListAsync();
        }
    }
}