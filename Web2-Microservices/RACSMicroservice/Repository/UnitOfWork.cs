using RACSMicroservice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly RACSContext context;

        public UnitOfWork(RACSContext context)
        {
            this.context = context;
        }

        private ICarRepository carRepository;
        private IBranchRepository branchRepository;
        private ICarRentRepository carRentRepository;
        private IRentCarServiceRepository rentCarServiceRepository;
        private IRACSSpecialOffer racsSpecialOfferRepository;
        public IRACSSpecialOffer RACSSpecialOfferRepository
        {
            get
            {
                return racsSpecialOfferRepository = racsSpecialOfferRepository ??
                    new RACSSpecialOfferRepository(this.context);
            }
        }
        public IRentCarServiceRepository RentCarServiceRepository
        {
            get
            {
                return rentCarServiceRepository = rentCarServiceRepository ??
                    new RentCarServiceRepository(this.context);
            }
        }
        public ICarRepository CarRepository
        {
            get
            {
                return carRepository = carRepository ??
                    new CarRepository(this.context);
            }
        }

        public IBranchRepository BranchRepository
        {
            get
            {
                return branchRepository = branchRepository ??
                    new BranchRepository(this.context);
            }
        }

        public ICarRentRepository CarRentRepository
        {
            get
            {
                return carRentRepository = carRentRepository ??
                    new CarRentRepository(this.context);
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
