using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public interface IUnitOfWork
    {
        IAirlineRepository AirlineRepository { get; }
        IDestinationRepository DestinationRepository { get; }
        ISpecialOfferRepository SpecialOfferRepository { get; }
        IFlightRepository FlightRepository { get; }
        ITicketRepository TicketRepository { get; }
        ISeatRepository SeatRepository { get; }
        IFlightReservationRepository FlightReservationRepository { get; }
        Task Commit();
        void Rollback();
    }
}
