using ReservationsMicroservice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly ReservationsContext context;

        public UnitOfWork(ReservationsContext context)
        {
            this.context = context;
        }

        private IFlightReservationRepository flightReservationRepository;
        public IFlightReservationRepository FlightReservationRepository
        {
            get
            {
                return flightReservationRepository = flightReservationRepository ??
                    new FlightReservationRepository(this.context);
            }
        }

        private ICarReservationRepository carReservationRepository;
        public ICarReservationRepository CarReservationRepository
        {
            get
            {
                return carReservationRepository = carReservationRepository ??
                    new CarReservationRepository(this.context);
            }
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
    }
}
