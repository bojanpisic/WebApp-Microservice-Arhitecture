using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Data
{
    public class UserContext : IdentityDbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
                                                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ////friendship realtions
            modelBuilder.Entity<Friendship>()
                .HasKey(bc => new { bc.User1Id, bc.User2Id });
            modelBuilder.Entity<Friendship>()
                .HasOne(bc => bc.User1)
                .WithMany(b => b.FriendshipInvitations)
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Friendship>()
                .HasOne(bc => bc.User2)
                .WithMany(c => c.FriendshipRequests)
                .HasForeignKey(bc => bc.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            //trip invitation realtions
            modelBuilder.Entity<User>()
               .HasMany(c => c.TripRequests)
               .WithOne(e => e.Receiver);
            modelBuilder.Entity<User>()
               .HasMany(c => c.TripInvitations)
               .WithOne(e => e.Sender);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Invitation> Invitations { get; set; }


    }
}
