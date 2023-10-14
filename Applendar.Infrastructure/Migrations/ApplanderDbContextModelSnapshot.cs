﻿// <auto-generated />
using System;
using Applander.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Applander.Infrastructure.Migrations
{
    [DbContext(typeof(ApplanderDbContext))]
    partial class ApplanderDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Applander.Domain.Entities.ApplendarUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ArchivedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ApplendarUsers");
                });

            modelBuilder.Entity("Applander.Domain.Entities.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ArchivedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("IsCompanionAllowed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPetAllowed")
                        .HasColumnType("bit");

                    b.Property<int?>("MaximumNumberOfParticipants")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("OrganizerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("OrganizerId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("Applander.Domain.Entities.EventInvitation", b =>
                {
                    b.Property<Guid>("ApplendarUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ArchivedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("ApplendarUserId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("EventInvitation");
                });

            modelBuilder.Entity("Applander.Domain.Entities.ApplendarUser", b =>
                {
                    b.OwnsOne("Applander.Domain.Entities.Preferences", "Preferences", b1 =>
                        {
                            b1.Property<Guid>("ApplendarUserId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<bool>("NotifyAboutEventsWithCompanions")
                                .HasColumnType("bit");

                            b1.Property<bool>("NotifyAboutEventsWithPets")
                                .HasColumnType("bit");

                            b1.Property<bool>("NotifyAboutOfflineEvents")
                                .HasColumnType("bit");

                            b1.Property<bool>("NotifyAboutOnlineEvents")
                                .HasColumnType("bit");

                            b1.HasKey("ApplendarUserId");

                            b1.ToTable("ApplendarUsers");

                            b1.WithOwner()
                                .HasForeignKey("ApplendarUserId");
                        });

                    b.Navigation("Preferences")
                        .IsRequired();
                });

            modelBuilder.Entity("Applander.Domain.Entities.Event", b =>
                {
                    b.HasOne("Applander.Domain.Entities.ApplendarUser", "Organizer")
                        .WithMany("OrganizedEvents")
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Applander.Domain.Entities.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("EventId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Address")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Country")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<bool>("IsOnline")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Url")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ZipCode")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EventId");

                            b1.ToTable("Event");

                            b1.WithOwner()
                                .HasForeignKey("EventId");
                        });

                    b.Navigation("Location")
                        .IsRequired();

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("Applander.Domain.Entities.EventInvitation", b =>
                {
                    b.HasOne("Applander.Domain.Entities.ApplendarUser", "ApplendarUser")
                        .WithMany("EventInvitations")
                        .HasForeignKey("ApplendarUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Applander.Domain.Entities.Event", "Event")
                        .WithMany("Invitations")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ApplendarUser");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Applander.Domain.Entities.ApplendarUser", b =>
                {
                    b.Navigation("EventInvitations");

                    b.Navigation("OrganizedEvents");
                });

            modelBuilder.Entity("Applander.Domain.Entities.Event", b =>
                {
                    b.Navigation("Invitations");
                });
#pragma warning restore 612, 618
        }
    }
}
