﻿// <auto-generated />
using CarStoreWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CarStoreWeb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191118091739_WithAuthorDeclaration")]
    partial class WithAuthorDeclaration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CarStoreRest.Models.Car", b =>
                {
                    b.Property<int>("CarID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author");

                    b.Property<string>("Brand");

                    b.Property<string>("Model");

                    b.Property<decimal>("Price");

                    b.HasKey("CarID");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("CarStoreRest.Models.Car", b =>
                {
                    b.OwnsOne("CarStoreRest.Models.CarDescription", "CarDescription", b1 =>
                        {
                            b1.Property<int>("CarID")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Color");

                            b1.Property<double>("EngineСapacity");

                            b1.Property<string>("FuelType");

                            b1.Property<int>("YearOfManufacture");

                            b1.HasKey("CarID");

                            b1.ToTable("Cars");

                            b1.HasOne("CarStoreRest.Models.Car")
                                .WithOne("CarDescription")
                                .HasForeignKey("CarStoreRest.Models.CarDescription", "CarID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}