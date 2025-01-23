using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy
                            .WithOrigins("*")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                );
            });

            // Add controllers and swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            

            string connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DefaultConnection");

            // Register DbContext
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionString));

            var app = builder.Build();

            // Apply migrations at startup
            ApplyMigrations(app);

            // Use CORS
            app.UseCors(MyAllowSpecificOrigins);

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Right now, it's the same logic in production
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            // Redirect root to swagger
            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/swagger");
                await Task.CompletedTask;
            });

            app.Run();
        }

        private static void ApplyMigrations(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                // Important: use the same DbContext type you registered above
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}