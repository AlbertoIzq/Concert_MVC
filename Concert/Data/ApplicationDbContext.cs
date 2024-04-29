using ConcertWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // To create a table for Category data model
        public DbSet<Category> Categories { get; set; }

        // To seed Category table with some data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Chanson française", DisplayOrder = 1 },
                new Category() { Id = 2, Name = "Disco polo", DisplayOrder = 2 },
                new Category() { Id = 3, Name = "Reggae", DisplayOrder = 3 }
            );
        }
    }
}
