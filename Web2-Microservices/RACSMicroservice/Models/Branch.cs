using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Models
{
    public class Branch
    {
        public int BranchId { get; set; }
        public int RentACarServiceId { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public RentACarService RentACarService { get; set; }
        public ICollection<Car> Cars { get; set; }

        public Branch()
        {
            Cars = new HashSet<Car>();
        }
    }
}
