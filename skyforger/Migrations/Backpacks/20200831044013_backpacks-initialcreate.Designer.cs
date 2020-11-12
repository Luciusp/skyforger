﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using skyforger.models.backpacks;

namespace skyforger.Migrations.Backpacks
{
    [DbContext(typeof(BackpacksContext))]
    [Migration("20200831044013_backpacks-initialcreate")]
    partial class backpacksinitialcreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("skyforger.models.backpacks.BackpackItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("GpValue")
                        .HasColumnType("REAL");

                    b.Property<string>("ItemName")
                        .HasColumnType("TEXT");

                    b.Property<string>("NotesEffect")
                        .HasColumnType("TEXT");

                    b.Property<string>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<float>("Quantity")
                        .HasColumnType("REAL");

                    b.Property<float>("Weight")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Backpacks");
                });
#pragma warning restore 612, 618
        }
    }
}
