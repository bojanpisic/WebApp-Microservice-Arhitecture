using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Data;
using UserMicroservice.Models;

namespace UserMicroservice.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {

        private UserContext context;
        private readonly UserManager<Person> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<Person> signInManager;

        private IAuthenticationRepository authenticationRepository;
        private IUserRepository userRepository;
        private IProfileRepository profileRepository;
        private ITripInvitationRepository tripInvitationRepository;
        private IBonusRepository bonusRepository;

        public UnitOfWork(UserContext _context, RoleManager<IdentityRole> _roleManager,
                        UserManager<Person> _userManager, SignInManager<Person> _signInManager)
        {
            this.context = _context;
            this.roleManager = _roleManager;
            this.userManager = _userManager;
            this.signInManager = _signInManager;
        }

        public UserManager<Person> UserManager
        {
            get => this.userManager;
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get => this.roleManager;
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Commit()
        {
            await context.SaveChangesAsync();
        }

        public void Rollback()
        {
            this.Dispose();
        }

        #region Repository getters
        public IBonusRepository BonusRepository
        {
            get
            {
                return bonusRepository = bonusRepository ??
                    new BonusRepository(this.context);
            }
        }
        public ITripInvitationRepository TripInvitationRepository
        {
            get
            {
                return tripInvitationRepository = tripInvitationRepository ?? new TripInvitationRepository(this.context);
            }
        }
        public IAuthenticationRepository AuthenticationRepository
        {
            get
            {
                return authenticationRepository = authenticationRepository ??
                    new AuthenticationRepository(this.userManager, this.roleManager);
            }
        }
        public IUserRepository UserRepository
        {
            get
            {
                return userRepository = userRepository ?? new UserRepository(this.context);
            }
        }
        public IProfileRepository ProfileRepository
        {
            get
            {
                return profileRepository = profileRepository ?? new ProfileRepository(this.context, this.userManager, this.signInManager);
            }
        }
        #endregion

    }
}
