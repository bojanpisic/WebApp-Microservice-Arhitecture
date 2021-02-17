using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class RACSAdmin : Person
    {
        public int RACSId { get; set; }
        public RACSAdmin()
        {
        }
    }
}
