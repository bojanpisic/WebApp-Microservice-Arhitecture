using Microsoft.EntityFrameworkCore;
using ReservationsMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationsMicroservice.Data
{
    public class ReservationsContext : DbContext
    {
        public ReservationsContext(DbContextOptions<ReservationsContext> options)
                                              : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlightReservation>()
                .HasMany(c => c.Tickets)
                .WithOne(e => e.Reservation);
            modelBuilder.Entity<FlightReservation>()
                .HasMany(c => c.UnregistredFriendsTickets)
                .WithOne(e => e.Reservation);

            modelBuilder.Entity<CarReservation>()
                .HasOne(a => a.FlightReservation)
                .WithOne(b => b.CarRent)
                .HasForeignKey<FlightReservation>(b => b.CarRentId);


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<FlightReservation> FlightReservations { get; set; }
        public DbSet<CarReservation> CarReservations { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Ticket2> Tickets2 { get; set; }
    }
}
