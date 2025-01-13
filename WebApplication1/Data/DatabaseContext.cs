using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data;

public class DatabaseContext : DbContext
{

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        // Add DBSet for each model
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Room> Rooms { get; set; }
        public DbSet<Models.Car> Cars { get; set; }
        
}