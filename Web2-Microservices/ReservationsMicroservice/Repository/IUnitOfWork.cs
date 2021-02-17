using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Repository
{
    public interface IUnitOfWork
    {
        IFlightReservationRepository FlightReservationRepository { get; }
        ICarReservationRepository CarReservationRepository { get; }
        Task Commit();
        void Rollback();
    }
}
