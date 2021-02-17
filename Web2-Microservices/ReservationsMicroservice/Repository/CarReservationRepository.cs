using ReservationsMicroservice.Data;
using ReservationsMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Repository
{
    public class CarReservationRepository: GenericRepository<CarReservation>, ICarReservationRepository
    {
        public CarReservationRepository(ReservationsContext context) : base(context)
        {
        }
    }
}
