using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class Bonus
    {
        public int BonusId { get; set; }
        public int BonusPerKilometer { get; set; }
        public int DiscountPerReservation { get; set; }

        public Bonus()
        {

        }
    }
}
