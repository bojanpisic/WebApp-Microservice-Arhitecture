using RACSMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RACSMicroservice.Repository
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<IEnumerable<Car>> AllCars(Expression<Func<Car, bool>> filter = null);
        Task<IEnumerable<Car>> CarsOfBranch(int id);
    }
}