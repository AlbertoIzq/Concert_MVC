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

        // To create a table for Category data model
        public DbSet<Category> Categories { get; set; }

        // To create a table for Product data model
        public DbSet<Product> Products { get; set; }

        // To seed Category table with some data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Chanson française", DisplayOrder = 1 },
                new Category() { Id = 2, Name = "Disco polo", DisplayOrder = 2 },
                new Category() { Id = 3, Name = "Reggae", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    Artist = "Ace of base",
                    Title = "All that she wants",
                    Length = new TimeSpan(0, 2, 6),
                    ReleaseYear = 1992
                },
                new Product()
                {
                    Id = 2,
                    Artist = "And one",
                    Title = "Military fashion show",
                    Length = new TimeSpan(0, 2, 19),
                    ReleaseYear = 2006
                },
                new Product()
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
