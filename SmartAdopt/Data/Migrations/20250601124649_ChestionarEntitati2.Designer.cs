﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartAdopt.Data;

#nullable disable

namespace SmartAdopt.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250601124649_ChestionarEntitati2")]
    partial class ChestionarEntitati2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SmartAdopt.Models.Admin", b =>
                {
                    b.Property<int>("idAdmin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idAdmin"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("idAdmin");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("SmartAdopt.Models.Animal", b =>
                {
                    b.Property<int>("idAnimal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idAnimal"));

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("descriere")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("grupa_varsta")
                        .HasColumnType("int");

                    b.Property<int>("marime")
                        .HasColumnType("int");

                    b.Property<int>("nivel_adaptabilitate")
                        .HasColumnType("int");

                    b.Property<int>("nivel_atentie_necesara")
                        .HasColumnType("int");

                    b.Property<int>("nivel_energie")
                        .HasColumnType("int");

                    b.Property<string>("nume")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("pret")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("rasa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("specie")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("starea_sanatatii")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tip_alimentatie")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ultima_verificare_vet")
                        .HasColumnType("datetime2");

                    b.Property<string>("vaccinuri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("varsta")
                        .HasColumnType("int");

                    b.HasKey("idAnimal");

                    b.ToTable("Animals");
                });

            modelBuilder.Entity("SmartAdopt.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("nume")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("prenume")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("SmartAdopt.Models.Client", b =>
                {
                    b.Property<int>("idClient")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idClient"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("CompletedProfile")
                        .HasColumnType("bit");

                    b.Property<int?>("RaspChestionaridRasp1")
                        .HasColumnType("int");

                    b.Property<string>("adresa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("idRaspChestionar")
                        .HasColumnType("int");

                    b.Property<string>("nr_telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idClient");

                    b.HasIndex("ApplicationUserId")
                        .IsUnique();

                    b.HasIndex("RaspChestionaridRasp1");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("SmartAdopt.Models.Comanda", b =>
                {
                    b.Property<int>("idComanda")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idComanda"));

                    b.Property<int?>("AnimalidAnimal")
                        .HasColumnType("int");

                    b.Property<int?>("ClientidClient")
                        .HasColumnType("int");

                    b.Property<DateTime>("data_comenzii")
                        .HasColumnType("datetime2");

                    b.Property<int>("idAnimal")
                        .HasColumnType("int");

                    b.Property<int>("idClient")
                        .HasColumnType("int");

                    b.Property<string>("metoda_platii")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("motivatie")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("stare")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("total_plata")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("idComanda");

                    b.HasIndex("AnimalidAnimal");

                    b.HasIndex("ClientidClient");

                    b.HasIndex("idAnimal");

                    b.HasIndex("idClient");

                    b.ToTable("Comandas");
                });

            modelBuilder.Entity("SmartAdopt.Models.Comentariu", b =>
                {
                    b.Property<int>("idComentariu")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idComentariu"));

                    b.Property<string>("descriere")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("idClient")
                        .HasColumnType("int");

                    b.Property<int>("idPostare")
                        .HasColumnType("int");

                    b.HasKey("idComentariu");

                    b.HasIndex("idClient");

                    b.HasIndex("idPostare");

                    b.ToTable("Comentarius");
                });

            modelBuilder.Entity("SmartAdopt.Models.Postare", b =>
                {
                    b.Property<int>("idPostare")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idPostare"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("data_postarii")
                        .HasColumnType("datetime2");

                    b.Property<string>("descriere")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("titlu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idPostare");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Postares");
                });

            modelBuilder.Entity("SmartAdopt.Models.RaspAnimal", b =>
                {
                    b.Property<int>("idRaspAnimal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idRaspAnimal"));

                    b.Property<int>("idAnimal")
                        .HasColumnType("int");

                    b.Property<int>("idRasp")
                        .HasColumnType("int");

                    b.HasKey("idRaspAnimal");

                    b.HasIndex("idAnimal");

                    b.HasIndex("idRasp");

                    b.ToTable("RaspAnimals");
                });

            modelBuilder.Entity("SmartAdopt.Models.RaspChestionar", b =>
                {
                    b.Property<int>("idRasp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idRasp"));

                    b.Property<int>("idClient")
                        .HasColumnType("int");

                    b.HasKey("idRasp");

                    b.HasIndex("idClient")
                        .IsUnique();

                    b.ToTable("RaspChestionars");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SmartAdopt.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SmartAdopt.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartAdopt.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SmartAdopt.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartAdopt.Models.Admin", b =>
                {
                    b.HasOne("SmartAdopt.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("SmartAdopt.Models.Client", b =>
                {
                    b.HasOne("SmartAdopt.Models.ApplicationUser", "ApplicationUser")
                        .WithOne()
                        .HasForeignKey("SmartAdopt.Models.Client", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartAdopt.Models.RaspChestionar", "RaspChestionar")
                        .WithMany()
                        .HasForeignKey("RaspChestionaridRasp1");

                    b.Navigation("ApplicationUser");

                    b.Navigation("RaspChestionar");
                });

            modelBuilder.Entity("SmartAdopt.Models.Comanda", b =>
                {
                    b.HasOne("SmartAdopt.Models.Animal", null)
                        .WithMany("Comandas")
                        .HasForeignKey("AnimalidAnimal");

                    b.HasOne("SmartAdopt.Models.Client", null)
                        .WithMany("Comandas")
                        .HasForeignKey("ClientidClient");

                    b.HasOne("SmartAdopt.Models.Animal", "Animal")
                        .WithMany()
                        .HasForeignKey("idAnimal")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartAdopt.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("idClient")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("SmartAdopt.Models.Comentariu", b =>
                {
                    b.HasOne("SmartAdopt.Models.Client", "Client")
                        .WithMany("Comentarius")
                        .HasForeignKey("idClient")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartAdopt.Models.Postare", "Postare")
                        .WithMany("Comentarius")
                        .HasForeignKey("idPostare")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Postare");
                });

            modelBuilder.Entity("SmartAdopt.Models.Postare", b =>
                {
                    b.HasOne("SmartAdopt.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("Postares")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("SmartAdopt.Models.RaspAnimal", b =>
                {
                    b.HasOne("SmartAdopt.Models.Animal", "Animal")
                        .WithMany("RaspAnimals")
                        .HasForeignKey("idAnimal")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartAdopt.Models.RaspChestionar", "RaspChestionar")
                        .WithMany("RaspAnimals")
                        .HasForeignKey("idRasp")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Animal");

                    b.Navigation("RaspChestionar");
                });

            modelBuilder.Entity("SmartAdopt.Models.RaspChestionar", b =>
                {
                    b.HasOne("SmartAdopt.Models.Client", "Client")
                        .WithOne()
                        .HasForeignKey("SmartAdopt.Models.RaspChestionar", "idClient")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("SmartAdopt.Models.Animal", b =>
                {
                    b.Navigation("Comandas");

                    b.Navigation("RaspAnimals");
                });

            modelBuilder.Entity("SmartAdopt.Models.ApplicationUser", b =>
                {
                    b.Navigation("Postares");
                });

            modelBuilder.Entity("SmartAdopt.Models.Client", b =>
                {
                    b.Navigation("Comandas");

                    b.Navigation("Comentarius");
                });

            modelBuilder.Entity("SmartAdopt.Models.Postare", b =>
                {
                    b.Navigation("Comentarius");
                });

            modelBuilder.Entity("SmartAdopt.Models.RaspChestionar", b =>
                {
                    b.Navigation("RaspAnimals");
                });
#pragma warning restore 612, 618
        }
    }
}
