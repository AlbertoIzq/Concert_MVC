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
    }
}