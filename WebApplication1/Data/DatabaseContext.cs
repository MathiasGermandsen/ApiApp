using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class DatabaseContext : DbContext
{
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        
        public DbSet<Models.User> User { get; set; }
        public DbSet<Models.Room> Rooms { get; set; }
        public DbSet<Models.Car> Car { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                // Configure the relationship: One User has many Cars
                modelBuilder.Entity<User>()
                        .HasMany(u => u.Cars)
                        .WithOne(c => c.User)
                        .HasForeignKey(c => c.UserId);

                base.OnModelCreating(modelBuilder);
        }
}