using Concert.Models;
using Microsoft.EntityFrameworkCore;

namespace Concert.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // To create a table for Genre data model
        public DbSet<Genre> Genres { get; set; }
        
        // To create a table for Song data model
        public DbSet<Song> Songs { get; set; }
        
        // To seed Genre table with some data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new Genre() { Id = 1, Name = "Chanson française", DisplayOrder = 1 },
                new Genre() { Id = 2, Name = "Disco polo", DisplayOrder = 2 },
                new Genre() { Id = 3, Name = "Reggae", DisplayOrder = 3 }
            );
            
            modelBuilder.Entity<Song>().HasData(
                new Song()
                {
                    Id = 1,
                    Artist = "Ace of base",
                    Title = "All that she wants",
                    Length = new TimeSpan(0, 2, 6),
                    ReleaseYear = 1992
                },
                new Song()
                {
                    Id = 2,
                    Artist = "And one",
                    Title = "Military fashion show",
                    Length = new TimeSpan(0, 2, 19),
                    ReleaseYear = 2006
                },
                new Song()
                {
                    Id = 3,
                    Artist = "Boys",
                    Title = "Szalona",
                    Length = new TimeSpan(0, 2, 52),
                    ReleaseYear = 1997
                }
            );
        }
    }
}
