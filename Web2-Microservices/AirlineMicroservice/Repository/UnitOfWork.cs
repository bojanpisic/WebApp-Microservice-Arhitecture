using AirlineMicroservice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private AirlineContext context;

        private IAirlineRepository airlineRepository;
        private IFlightRepository flightRepository;
        private IDestinationRepository destinationRepository;
        private ISeatRepository seatRepository;
        private ISpecialOfferRepository specialOfferRepository;
        private ITicketRepository ticketRepository;
        private IFlightReservationRepository flightReservationRepository;

        public UnitOfWork(AirlineContext _context)
        {
            this.context = _context;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Commit()
        {
            await context.SaveChangesAsync();
        }

        public void Rollback()
        {
            this.Dispose();
        }

        #region Repository getters

        public IAirlineRepository AirlineRepository
        {
            get
            {
                return airlineRepository = airlineRepository ?? new AirlineRepository(this.context);
            }
        }
        public IFlightRepository FlightRepository
        {
            get
            {
                return flightRepository = flightRepository ?? new FlightRepository(this.context);
            }
        }

        public ITicketRepository TicketRepository
        {
            get
            {
                return ticketRepository = ticketRepository ?? new TicketRepository(this.context);
            }
        }

        public ISeatRepository SeatRepository
        {
            get
            {

                return seatRepository = seatRepository ?? new SeatRepository(this.context);

            }
        }
        public IDestinationRepository DestinationRepository
        {
            get
            {
                return destinationRepository = destinationRepository ?? new DestinationRepository(this.context);
            }
        }

        public ISpecialOfferRepository SpecialOfferRepository
        {
            get
            {
                return specialOfferRepository = specialOfferRepository ?? new SpecialOfferRepository(this.context);
            }
        }

        public IFlightReservationRepository FlightReservationRepository
        {
            get
            {
                return flightReservationRepository = flightReservationRepository ?? new FlightReservationRepository(this.context);
            }
        }
        #endregion
    }
}
