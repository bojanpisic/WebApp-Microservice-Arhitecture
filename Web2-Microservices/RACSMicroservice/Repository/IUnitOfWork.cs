using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public interface IUnitOfWork
    {
        IBranchRepository BranchRepository { get; }
        ICarRentRepository CarRentRepository { get; }
        ICarRepository CarRepository { get; }

        IRACSSpecialOffer RACSSpecialOfferRepository { get; }
        IRentCarServiceRepository RentCarServiceRepository { get; }
        Task Commit();
        void Rollback();
    }
}
