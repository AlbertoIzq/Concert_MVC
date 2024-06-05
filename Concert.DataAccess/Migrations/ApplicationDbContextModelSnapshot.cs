﻿// <auto-generated />
using System;
using Concert.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Concert.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Concert.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Genres");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Disco polo"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "EBM"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Reggae"
                        });
                });

            modelBuilder.Entity("Concert.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Languages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "English"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "Polish"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "French"
                        });
                });

            modelBuilder.Entity("Concert.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Length")
                        .HasColumnType("time");

                    b.Property<int>("ReleaseYear")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("GenreId");

                    b.HasIndex("LanguageId");

                    b.ToTable("Songs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Artist = "Ace of base",
                            GenreId = 3,
                            ImageUrl = "",
                            LanguageId = 1,
                            Length = new TimeSpan(0, 0, 2, 6, 0),
                            ReleaseYear = 1992,
                            Title = "All that she wants"
                        },
                        new
                        {
                            Id = 2,
                            Artist = "And one",
                            GenreId = 2,
                            ImageUrl = "",
                            LanguageId = 1,
                            Length = new TimeSpan(0, 0, 2, 19, 0),
                            ReleaseYear = 2006,
                            Title = "Military fashion show"
                        },
                        new
                        {
                            Id = 3,
                            Artist = "Boys",
                            GenreId = 1,
                            ImageUrl = "",
                            LanguageId = 3,
                            Length = new TimeSpan(0, 0, 2, 52, 0),
                            ReleaseYear = 1997,
                            Title = "Szalona"
                        });
                });

            modelBuilder.Entity("Concert.Models.Song", b =>
                {
                    b.HasOne("Concert.Models.Genre", "Genre")
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Concert.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Language");
                });
#pragma warning restore 612, 618
        }
    }
}
