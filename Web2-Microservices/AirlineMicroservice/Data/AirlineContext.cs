using AirlineMicroservice.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.Data
{
    public class AirlineContext : IdentityDbContext
    {
        public AirlineContext(DbContextOptions<AirlineContext> options)
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

            //airline rates -> one to many
            modelBuilder.Entity<Airline>()
               .HasMany(c => c.Rates)
               .WithOne(e => e.Airline);
            modelBuilder.Entity<Flight>()
               .HasMany(c => c.Rates)
               .WithOne(e => e.Flight);        

            //airline-destinations
            modelBuilder.Entity<AirlineDestination>()
               .HasKey(bc => new { bc.AirlineId, bc.DestinationId });
            modelBuilder.Entity<AirlineDestination>()
                .HasOne(bc => bc.Airline)
                .WithMany(b => b.Destinations)
                .HasForeignKey(bc => bc.AirlineId);
            modelBuilder.Entity<AirlineDestination>()
                .HasOne(bc => bc.Destination)
                .WithMany(c => c.Airlines)
                .HasForeignKey(bc => bc.DestinationId);

            //flight-address
            modelBuilder.Entity<FlightDestination>()
                .HasKey(bc => new { bc.DestinationId, bc.FlightId });
            modelBuilder.Entity<FlightDestination>()
                .HasOne(bc => bc.Flight)
                .WithMany(b => b.Stops)
                .HasForeignKey(bc => bc.FlightId);
            modelBuilder.Entity<FlightDestination>()
                .HasOne(bc => bc.Destination)
                .WithMany(c => c.Flights)
                .HasForeignKey(bc => bc.DestinationId);

            //flight-seats
            modelBuilder.Entity<Flight>()
               .HasMany(c => c.Seats)
               .WithOne(e => e.Flight);

            //flight-from-address
            modelBuilder.Entity<Destination>()
               .HasMany(c => c.From)
               .WithOne(e => e.From);
            //flight-from-address
            modelBuilder.Entity<Destination>()
               .HasMany(c => c.To)
               .WithOne(e => e.To);

            modelBuilder.Entity<Airline>()
               .HasMany(c => c.SpecialOffers)
               .WithOne(e => e.Airline);
            //specoff-seat
            modelBuilder.Entity<SpecialOffer>()
               .HasMany(c => c.Seats)
               .WithOne(e => e.SpecialOffer);
            //airline-flights
            modelBuilder.Entity<Airline>()
               .HasMany(c => c.Flights)
               .WithOne(e => e.Airline);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<FlightRate> FlightRates { get; set; }
        public DbSet<SpecialOffer> SpecialOffers { get; set; }
        public DbSet<AirlineDestination> AirlineDestination { get; set; }
        public DbSet<AirlineRate> AirlineRates { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightDestination> FlightsAddresses { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<FlightReservation> FlightReservations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Ticket2> Tickets2 { get; set; }

    }
}
