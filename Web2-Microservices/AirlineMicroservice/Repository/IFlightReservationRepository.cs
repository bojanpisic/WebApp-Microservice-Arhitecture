using AirlineMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface IFlightReservationRepository : IGenericRepository<FlightReservation>
    {
        Task<IEnumerable<FlightReservation>> GetTrips(string userId);
        Task<FlightReservation> GetReservationById(int flightReservationId);
    }
}
