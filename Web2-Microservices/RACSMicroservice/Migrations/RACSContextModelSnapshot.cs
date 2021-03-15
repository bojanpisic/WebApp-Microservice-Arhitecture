﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RACSMicroservice.Data;

namespace RACSMicroservice.Migrations
{
    [DbContext(typeof(RACSContext))]
    partial class RACSContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RACSMicroservice.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("Lat")
                        .HasColumnType("double");

                    b.Property<double>("Lon")
                        .HasColumnType("double");

                    b.Property<int>("RentACarServiceId")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("AddressId");

                    b.HasIndex("RentACarServiceId")
                        .IsUnique();

                    b.ToTable("Address");
                });

            modelBuilder.Entity("RACSMicroservice.Models.Branch", b =>
                {
                    b.Property<int>("BranchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("RentACarServiceId")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("BranchId");

                    b.HasIndex("RentACarServiceId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("RACSMicroservice.Models.Car", b =>
                {
                    b.Property<int>("CarId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("Brand")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<byte[]>("ImageUrl")
                        .HasColumnType("longblob");

                    b.Property<string>("Model")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<float>("PricePerDay")
                        .HasColumnType("float");

                    b.Property<int?>("RentACarServiceId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<int>("SeatsNumber")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("CarId");

                    b.HasIndex("BranchId");

                    b.HasIndex("RentACarServiceId");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("RACSMicroservice.Models.CarRate", b =>
                {
                    b.Property<int>("CarRateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CarId")
                        .HasColumnType("int");

                    b.Property<float>("Rate")
                        .HasColumnType("float");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("CarRateId");

                    b.HasIndex("CarId");

                    b.ToTable("CarRates");
                });

            modelBuilder.Entity("RACSMicroservice.Models.CarRent", b =>
                {
                    b.Property<int>("CarRentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("RentDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("RentedCarCarId")
                        .HasColumnType("int");

                    b.Property<string>("ReturnCity")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ReturnDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TakeOverCity")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("TakeOverDate")
                        .HasColumnType("datetime(6)");

                    b.Property<float>("TotalPrice")
                        .HasColumnType("float");

                    b.Property<int?>("TripReservationId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("CarRentId");

                    b.HasIndex("RentedCarCarId");

                    b.ToTable("CarRents");
                });

            modelBuilder.Entity("RACSMicroservice.Models.CarSpecialOffer", b =>
                {
                    b.Property<int>("CarSpecialOfferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CarId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsReserved")
                        .HasColumnType("tinyint(1)");

                    b.Property<float>("NewPrice")
                        .HasColumnType("float");

                    b.Property<float>("OldPrice")
                        .HasColumnType("float");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("CarSpecialOfferId");

                    b.HasIndex("CarId");

                    b.ToTable("CarSpecialOffers");
                });

            modelBuilder.Entity("RACSMicroservice.Models.RentACarService", b =>
                {
                    b.Property<int>("RentACarServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("About")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("AdminId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<byte[]>("LogoUrl")
                        .HasColumnType("longblob");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("RentACarServiceId");

                    b.ToTable("RentACarServices");
                });

            modelBuilder.Entity("RACSMicroservice.Models.RentCarServiceRates", b =>
                {
                    b.Property<int>("RentCarServiceRatesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Rate")
                        .HasColumnType("float");

                    b.Property<int?>("RentACarServiceId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("RentCarServiceRatesId");

                    b.HasIndex("RentACarServiceId");

                    b.ToTable("RentCarServiceRates");
                });

            modelBuilder.Entity("RACSMicroservice.Models.Address", b =>
                {
                    b.HasOne("RACSMicroservice.Models.RentACarService", "RentACarService")
                        .WithOne("Address")
                        .HasForeignKey("RACSMicroservice.Models.Address", "RentACarServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RACSMicroservice.Models.Branch", b =>
                {
                    b.HasOne("RACSMicroservice.Models.RentACarService", "RentACarService")
                        .WithMany("Branches")
                        .HasForeignKey("RentACarServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RACSMicroservice.Models.Car", b =>
                {
                    b.HasOne("RACSMicroservice.Models.Branch", "Branch")
                        .WithMany("Cars")
                        .HasForeignKey("BranchId");

                    b.HasOne("RACSMicroservice.Models.RentACarService", "RentACarService")
                        .WithMany("Cars")
                        .HasForeignKey("RentACarServiceId");
                });

            modelBuilder.Entity("RACSMicroservice.Models.CarRate", b =>
                {
                    b.HasOne("RACSMicroservice.Models.Car", "Car")
                        .WithMany("Rates")
                        .HasForeignKey("CarId");
                });

            modelBuilder.Entity("RACSMicroservice.Models.CarRent", b =>
                {
                    b.HasOne("RACSMicroservice.Models.Car", "RentedCar")
                        .WithMany("Rents")
                        .HasForeignKey("RentedCarCarId");
                });

            modelBuilder.Entity("RACSMicroservice.Models.CarSpecialOffer", b =>
                {
                    b.HasOne("RACSMicroservice.Models.Car", "Car")
                        .WithMany("SpecialOffers")
                        .HasForeignKey("CarId");
                });

            modelBuilder.Entity("RACSMicroservice.Models.RentCarServiceRates", b =>
                {
                    b.HasOne("RACSMicroservice.Models.RentACarService", "RentACarService")
                        .WithMany("Rates")
                        .HasForeignKey("RentACarServiceId");
                });
#pragma warning restore 612, 618
        }
    }
}
