﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Timescales.Models;

namespace Timescales.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20181210104426_2")]
    partial class _2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Timescales.Models.Audit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(7);

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("Days");

                    b.Property<DateTime>("OldestWorkDate");

                    b.Property<Guid>("TimescaleId");

                    b.Property<string>("User")
                        .IsRequired()
                        .IsFixedLength(true)
                        .HasMaxLength(7)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("TimescaleId");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("Timescales.Models.Timescale", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Basis")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<int>("Days");

                    b.Property<string>("LineOfBusiness")
                        .IsRequired()
                        .HasMaxLength(3);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<DateTime>("OldestWorkDate");

                    b.Property<string>("Owners")
                        .IsRequired();

                    b.Property<string>("Placeholder")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<string>("Site")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("Placeholder")
                        .IsUnique();

                    b.ToTable("Timescales");
                });

            modelBuilder.Entity("Timescales.Models.Audit", b =>
                {
                    b.HasOne("Timescales.Models.Timescale", "Timescale")
                        .WithMany("Audit")
                        .HasForeignKey("TimescaleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
