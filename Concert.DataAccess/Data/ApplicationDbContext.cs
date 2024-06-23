using Concert.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Concert.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // To create a table for each data model
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<SetList> SetLists { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        // To seed Genre table with some data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasData(
                new Genre() { Id = 1, Name = "Disco polo", DisplayOrder = 1 },
                new Genre() { Id = 2, Name = "EBM", DisplayOrder = 2 },
                new Genre() { Id = 3, Name = "Reggae", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Language>().HasData(
                new Language() { Id = 1, Name = "English", DisplayOrder = 1 },
                new Language() { Id = 2, Name = "Polish", DisplayOrder = 2 },
                new Language() { Id = 3, Name = "French", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Song>().HasData(
                new Song()
                {
                    Id = 1,
                    Artist = "Ace of base",
                    Title = "All that she wants",
                    Length = new TimeSpan(0, 2, 6),
                    ReleaseYear = 1992,
                    GenreId = 3,
                    LanguageId = 1,
                    ImageUrl = ""
                },
                new Song()
                {
                    Id = 2,
                    Artist = "And one",
                    Title = "Military fashion show",
                    Length = new TimeSpan(0, 2, 19),
                    ReleaseYear = 2006,
                    GenreId = 2,
                    LanguageId = 1,
                    ImageUrl = ""
                },
                new Song()
                {
                    Id = 3,
                    Artist = "Boys",
                    Title = "Szalona",
                    Length = new TimeSpan(0, 2, 52),
                    ReleaseYear = 1997,
                    GenreId = 1,
                    LanguageId = 3,
                    ImageUrl = ""
                }
            );

            modelBuilder.Entity<Company>().HasData(
               new Company()
               {
                   Id = 1,
                   Name = "Razzmatazz",
                   StreetAddress = "C/ dels Almogàvers, 122, Sant Martí",
                   City = "Barcelona",
                   State = "Catalunya",
                   Country = "Spain",
                   PostalCode = "08018",
                   PhoneNumber = "+34 933208200"
               },
               new Company()
               {
                   Id = 2,
                   Name = "Sala Salamandra",
                   StreetAddress = "Av. del Carrilet, 235",
                   City = "L'Hospitalet de Llobregat",
                   State = "Catalonia",
                   Country = "Spain",
                   PostalCode = "08907",
                   PhoneNumber = "+34 933370602"
               },
               new Company()
               {
                   Id = 3,
                   Name = "Botanique",
                   StreetAddress = "Blue Royale 236",
                   City = "Bruxelles",
                   State = "Bruxelles",
                   Country = "Belgium",
                   PostalCode = "1210",
                   PhoneNumber = "+32 022183732"
               }
            );
        }
    }
}
