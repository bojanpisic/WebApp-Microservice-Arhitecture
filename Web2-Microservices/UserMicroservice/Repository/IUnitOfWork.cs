using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public interface IUnitOfWork
    {
        IAuthenticationRepository AuthenticationRepository { get; }
        IUserRepository UserRepository { get; }
        IProfileRepository ProfileRepository { get; }
        ITripInvitationRepository TripInvitationRepository { get; }
        IBonusRepository BonusRepository { get; }

        UserManager<Person> UserManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        Task Commit();
        void Rollback();
    }
}
