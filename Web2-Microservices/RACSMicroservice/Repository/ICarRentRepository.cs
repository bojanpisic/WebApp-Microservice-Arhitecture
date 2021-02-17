using RACSMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public interface ICarRentRepository : IGenericRepository<CarRent>
    {
        Task<IEnumerable<CarRent>> GetRents(string userId);
        Task<CarRent> GetRentByFilter(Expression<Func<CarRent, bool>> filter = null);

    }
}