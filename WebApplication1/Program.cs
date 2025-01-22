using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // Specify when frontend is created
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    policy => { policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); }
                );
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            IConfiguration Configuration = builder.Configuration;
            // Configure Entity Framework with PostgreSQL
            string connectionString = Configuration.GetConnectionString("DefaultConnection")
                                      ?? Environment.GetEnvironmentVariable("DefaultConnection");

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionString));

            var app = builder.Build();

            app.UseCors(MyAllowSpecificOrigins);

            // Configure the HTTP request pipeline.
            // Swagger link: https://apiexampleproject.onrender.com/swagger/index.html
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();


            app.MapControllers();
            app.MapGet("/", async context => 
            {
                context.Response.Redirect("/swagger");
                await Task.CompletedTask;
            });
            
            app.Run();
        }
    }
        
        private static void ApplyMigrations(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("Applying pending migrations...");
                    dbContext.Database.Migrate();
                    Console.WriteLine("Migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("No pending migrations found.");
                }
            }
        }
    }
}
