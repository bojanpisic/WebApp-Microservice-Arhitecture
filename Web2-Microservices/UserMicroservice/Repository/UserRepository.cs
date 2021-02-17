using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Data;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public class UserRepository : GenericRepository<Person>, IUserRepository
    {
        public UserRepository(UserContext context) : base(context)
        {
        }

        //public async Task<IdentityResult> CreateFriendshipInvitation(Person sender, Person receiver)
        //{
        //    //using (var transaction = context.Database.BeginTransactionAsync())
        //    //{
        //        try
        //        {
        //            //flight.Stops = new List<FlightDestination>
        //            //{
        //            //    new FlightDestination{
        //            //        Flight = flight,
        //            //        Destination = stop
        //            //    }
        //            //};
        //            User s = (User)sender;
        //            User r = (User)receiver;
        //            var f = new Friendship() {Rejacted = false, Accepted = false, User1 = s, User2 = r };

        //            s.FriendshipInvitations.Add(f);
        //            r.FriendshipRequests.Add(f);

        //            this.UpdateUser(s);
        //            this.UpdateUser(r);
        //        //await transaction.Result.CommitAsync();
        //            unitOfWork.Commit();
        //            return IdentityResult.Success;
        //        }
        //        catch (Exception)
        //        {
        //        //await transaction.Result.RollbackAsync();

        //            return IdentityResult.Failed();
        //        }
        //    //}
        //}

        public void DeleteFriendship(Friendship friendship)
        {
            context.Friendships.Remove(friendship);
        }

        public async Task<IEnumerable<Person>> GetAllUsers()
        {
            return await context.Persons.ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetFriends(User user)
        {
            return await context.Friendships
                .Include(f => f.User2)
                .Include(f => f.User1)
                .Where(f => (f.User1 == user || f.User2 == user) && f.Accepted).ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetInvitations(Person user)
        {
            return await context.Friendships
                .Include(f => f.User2)
                .Include(f => f.User1)
                .Where(f => f.User1Id == user.Id || f.User2Id == user.Id).ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetRequests(Person user)
        {
            return await context.Friendships.Include(f => f.User1).Where(f => f.User2Id == user.Id).ToListAsync();
        }

        public async Task<Friendship> GetSpecificRequest(string user, string inviteSender)
        {
            return await context.Friendships
                .Include(f => f.User1)
                .Include(f => f.User2)
                .FirstOrDefaultAsync(f => f.User1Id == inviteSender && f.User2Id == user
                                    || f.User2Id == inviteSender && f.User1Id == user);
        }


    }
}
