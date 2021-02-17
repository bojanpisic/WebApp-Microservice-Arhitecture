using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RACSMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.Data
{
    public class RACSContext : IdentityDbContext
    {
        public RACSContext(DbContextOptions<RACSContext> options)
                                                  : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //rates
            modelBuilder.Entity<RentACarService>()
              .HasMany(c => c.Rates)
              .WithOne(e => e.RentACarService);
            modelBuilder.Entity<Car>()
               .HasMany(c => c.Rates)
               .WithOne(e => e.Car);
            // racs - address
            modelBuilder.Entity<RentACarService>()
               .HasOne(a => a.Address)
               .WithOne(b => b.RentACarService)
               .HasForeignKey<Address>(b => b.RentACarServiceId);

            //racservice-branch
            modelBuilder.Entity<RentACarService>()
               .HasMany(c => c.Branches)
               .WithOne(e => e.RentACarService);
            //racservice-car
            modelBuilder.Entity<RentACarService>()
               .HasMany(c => c.Cars)
               .WithOne(e => e.RentACarService);
            //branch-car
            modelBuilder.Entity<Branch>()
               .HasMany(c => c.Cars)
               .WithOne(e => e.Branch);
            //car - specoff
            modelBuilder.Entity<Car>()
               .HasMany(c => c.SpecialOffers)
               .WithOne(e => e.Car);
            modelBuilder.Entity<Car>()
               .HasMany(c => c.Rents)
               .WithOne(e => e.RentedCar);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<RentACarService> RentACarServices { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<RentCarServiceRates> RentCarServiceRates { get; set; }
        public DbSet<CarSpecialOffer> CarSpecialOffers { get; set; }
        public DbSet<CarRent> CarRents { get; set; }
        public DbSet<CarRate> CarRates { get; set; }
    }
}
