using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Data;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public class BonusRepository : GenericRepository<Bonus>, IBonusRepository
    {
        public BonusRepository(UserContext context) : base(context)
        {
        }
    }
}
