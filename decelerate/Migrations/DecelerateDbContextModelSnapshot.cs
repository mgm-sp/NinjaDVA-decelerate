﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using decelerate.Models;

namespace decelerate.Migrations
{
    [DbContext(typeof(DecelerateDbContext))]
    partial class DecelerateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0");

            modelBuilder.Entity("decelerate.Models.User", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SpeedChoice")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
