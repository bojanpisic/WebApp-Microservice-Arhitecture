using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Models
{
    public class Friendship
    {
        public string User1Id { get; set; }
        public User User1 { get; set; }

        public string User2Id { get; set; }
        public User User2 { get; set; }
        public bool Accepted { get; set; }
        public bool Rejacted { get; set; }

        public Friendship()
        {
            Accepted = false;
            Rejacted = false;
        }
    }
}
