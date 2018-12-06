﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using challenges.Models;

namespace challenges.Migrations
{
    [DbContext(typeof(challengesContext))]
    [Migration("20181204163713_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("challenges.Models.Activity", b =>
                {
                    b.Property<int>("ActivityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivityName");

                    b.Property<string>("GoalMetric");

                    b.HasKey("ActivityId");

                    b.ToTable("Activity");
                });

            modelBuilder.Entity("challenges.Models.Challenge", b =>
                {
                    b.Property<int>("ChallengeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityId");

                    b.Property<DateTime>("EndDateTime");

                    b.Property<int>("Goal");

                    b.Property<string>("Groupid");

                    b.Property<bool>("Repeat");

                    b.Property<DateTime>("StartDateTime");

                    b.Property<bool>("isGroupChallenge");

                    b.HasKey("ChallengeId");

                    b.HasIndex("ActivityId");

                    b.ToTable("Challenge");
                });

            modelBuilder.Entity("challenges.Models.UserChallenge", b =>
                {
                    b.Property<int>("UserChallengeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChallengeId");

                    b.Property<int>("PercentageComplete");

                    b.Property<string>("UserId");

                    b.HasKey("UserChallengeId");

                    b.HasIndex("ChallengeId");

                    b.ToTable("UserChallenge");
                });

            modelBuilder.Entity("challenges.Models.Challenge", b =>
                {
                    b.HasOne("challenges.Models.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("challenges.Models.UserChallenge", b =>
                {
                    b.HasOne("challenges.Models.Challenge", "Challenge")
                        .WithMany()
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
