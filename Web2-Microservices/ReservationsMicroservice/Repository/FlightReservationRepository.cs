using ReservationsMicroservice.Data;
using ReservationsMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Repository
{
    public class FlightReservationRepository : GenericRepository<FlightReservation>, IFlightReservationRepository
    {
        public FlightReservationRepository(ReservationsContext context) : base(context)
        {
        }
    }
}
