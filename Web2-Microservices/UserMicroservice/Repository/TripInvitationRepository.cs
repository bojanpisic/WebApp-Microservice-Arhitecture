using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Data;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public class TripInvitationRepository : GenericRepository<Invitation>, ITripInvitationRepository
    {
        public TripInvitationRepository(UserContext context) : base(context)
        {
        }

        public async Task<Invitation> GetTripInvitationById(int id)
        {
            return await context.Invitations
                //.Include(i => i.Seat)
                //.ThenInclude(s => s.Flight)
                .Include(i => i.Sender)
                .FirstOrDefaultAsync(i => i.InvitationId == id);
        }

        public async Task<IEnumerable<Invitation>> GetTripInvitations(User user)
        {
            return await context.Invitations
                //.Include(i => i.Seat)
                //.ThenInclude(s => s.Flight)
                //.ThenInclude(f => f.From)
                //.Include(i => i.Seat)
                //.ThenInclude(s => s.Flight)
                //.ThenInclude(f => f.To)
                //.Include(i => i.Seat)
                //.ThenInclude(s => s.Flight)
                //.ThenInclude(ff => ff.Airline)
                .Include(i => i.Sender)
                .Where(t => t.Receiver == user)
                .ToListAsync();
        }
    }
}